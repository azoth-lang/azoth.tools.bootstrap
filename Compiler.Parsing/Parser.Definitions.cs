using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    public IFixedList<INonMemberDefinitionSyntax> ParseNonMemberDefinitions<T>()
        where T : IToken
    {
        var declarations = new List<INonMemberDefinitionSyntax>();
        // Stop at end of file or some token that contains these declarations
        while (!Tokens.AtEnd<T>())
            try
            {
                declarations.Add(ParseNonMemberDefinition());
            }
            catch (ParseFailedException)
            {
                // Ignore: we would have consumed something before failing, try to get the next declaration
            }

        return declarations.ToFixedList();
    }

    public INonMemberDefinitionSyntax ParseNonMemberDefinition()
    {
        var attributes = ParseAttributes();
        var modifiers = ParseModifiers();

        switch (Tokens.Current)
        {
            case INamespaceKeywordToken _:
                return ParseNamespace(modifiers);
            case IClassKeywordToken _:
                return ParseClass(modifiers);
            case IStructKeywordToken _:
                return ParseStruct(modifiers);
            case ITraitKeywordToken _:
                return ParseTrait(modifiers);
            case IFunctionKeywordToken _:
                return ParseFunction(attributes, modifiers);
            default:
                Tokens.UnexpectedToken();
                throw new ParseFailedException();
        }
    }

    [SuppressMessage("Performance", "CA1826:Do not use Enumerable methods on indexable collections. Instead use the collection directly",
        Justification = "LastOrDefault() doesn't have a simple equivalent")]
    private ModifierParser ParseModifiers()
    {
        var modifiers = AcceptMany(Tokens.AcceptToken<IModifierToken>);
        var endOfModifiers = TokenFactory.EndOfFile(modifiers.LastOrDefault()?.Span.AtEnd() ?? Tokens.Current.Span.AtStart());
        var modifierTokens = new TokenIterator<IEssentialToken>(Tokens.Context, modifiers.Append<IEssentialToken>(endOfModifiers));
        return new ModifierParser(modifierTokens);
    }

    private IFixedList<IStandardTypeNameSyntax> ParseSupertypes()
        => Tokens.Accept<ILessThanColonToken>()
            ? ParseStandardTypeNames()
            : FixedList.Empty<IStandardTypeNameSyntax>();

    #region Parse Namespaces
    internal NamespaceDefinitionSyntax ParseNamespace(ModifierParser modifiers)
    {
        modifiers.ParseEndOfModifiers();
        var ns = Tokens.Consume<INamespaceKeywordToken>();
        IPunctuationToken? globalQualifier = Tokens.AcceptToken<IColonColonToken>();
        NamespaceName name = NamespaceName.Global;
        TextSpan nameSpan;
        if (globalQualifier is null)
        {
            globalQualifier = Tokens.AcceptToken<IColonColonDotToken>();
            (name, nameSpan) = ParseNamespaceName();
            nameSpan = TextSpan.Covering(globalQualifier?.Span, nameSpan);
        }
        else
            nameSpan = globalQualifier.Span;
        Tokens.Expect<IOpenBraceToken>();
        var usingDirectives = ParseUsingDirectives();
        var declarations = ParseNonMemberDefinitions<ICloseBraceToken>();
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(ns, closeBrace);
        return new NamespaceDefinitionSyntax(span, File, globalQualifier is not null, name, nameSpan, usingDirectives, declarations);
    }

    private (NamespaceName, TextSpan) ParseNamespaceName()
    {
        var firstSegment = Tokens.RequiredToken<IIdentifierToken>();
        var span = firstSegment.Span;
        NamespaceName name = firstSegment.Value;

        while (Tokens.Accept<IDotToken>())
        {
            var (nameSegment, segmentSpan) = Tokens.ExpectToken<IIdentifierToken>();
            // We need the span to cover a trailing dot
            span = TextSpan.Covering(span, segmentSpan);
            if (nameSegment is null)
                break;
            name = name.Qualify(nameSegment.Value);
        }

        return (name, span);
    }
    #endregion

    #region Parse Functions
    internal IFunctionDefinitionSyntax ParseFunction(
        IFixedList<IAttributeSyntax> attributes,
        ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var fn = Tokens.Consume<IFunctionKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        var parameters = ParseParameters(ParseFunctionParameter);
        var @return = ParseReturn();
        var body = ParseBody();
        var span = TextSpan.Covering(accessModifer?.Span, fn, body.Span);
        return new FunctionDefinitionSyntax(span, File, attributes,
            accessModifer, identifier.Span, name, parameters, @return, body);
    }

    private IFixedList<TParameter> ParseParameters<TParameter>(Func<TParameter> parseParameter)
        where TParameter : class, IParameterSyntax
    {
        Tokens.Expect<IOpenParenToken>();
        var parameters = ParseManySeparated<TParameter, ICommaToken, ICloseParenToken>(parseParameter);
        Tokens.Expect<ICloseParenToken>();
        return parameters.ToFixedList();
    }

    private IBodySyntax ParseBody()
        => Tokens.Current is IRightDoubleArrowToken ? ParseExpressionBody() : ParseBlockBody();

    private IExpressionBodySyntax ParseExpressionBody()
    {
        var resultStatement = ParseResultStatement();
        return new ExpressionBodySyntax(resultStatement.Span, resultStatement);
    }

    private IBlockBodySyntax ParseBlockBody()
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var statements = ParseMany<IStatementSyntax, ICloseBraceToken>(ParseStatement);
        foreach (var resultStatement in statements.OfType<IResultStatementSyntax>())
            Add(ParseError.ResultStatementInBody(File, resultStatement.Span));
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return new BlockBodySyntax(span, statements.OfType<IBodyStatementSyntax>().ToFixedList());
    }

    private IReturnSyntax? ParseReturn()
    {
        var rightArrow = Tokens.AcceptToken<IRightArrowToken>();
        if (rightArrow is null)
            return null;
        var type = ParseType();
        var span = TextSpan.Covering(rightArrow.Span, type.Span);
        return new ReturnSyntax(span, type);
    }
    #endregion

    #region Parse Class Declarations
    private IClassDefinitionSyntax ParseClass(
        ModifierParser modifiers)
    {
        var accessModifier = modifiers.ParseAccessModifier();
        var abstractModifier = modifiers.ParseAbstractModifier();
        var constModifier = modifiers.ParseConstModifier();
        var moveModifier = modifiers.ParseMoveModifier();
        modifiers.ParseEndOfModifiers();
        var classKeywordSpan = Tokens.Consume<IClassKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var name = identifier.Value;
        var generic = AcceptGenericParameters();
        var genericParameters = generic?.Parameters ?? FixedList.Empty<IGenericParameterSyntax>();
        IStandardTypeNameSyntax? baseClass = null;
        if (Tokens.Accept<IColonToken>()) baseClass = ParseStandardTypeName();
        var superTypes = ParseSupertypes();
        var (members, bodySpan) = ParseClassBody();
        var span = TextSpan.Covering(classKeywordSpan, identifier.Span, generic?.Span, baseClass?.Span,
            TextSpan.Covering(superTypes.Select(st => st.Span)), bodySpan);
        // TODO parse nested traits
        return new ClassDefinitionSyntax(span, File, accessModifier,
            abstractModifier, constModifier, moveModifier, identifier.Span, name, genericParameters,
            baseClass, superTypes, members);
    }

    private (IFixedList<IGenericParameterSyntax> Parameters, TextSpan Span)? AcceptGenericParameters()
    {
        var openBracket = Tokens.AcceptToken<IOpenBracketToken>();
        if (openBracket is null) return null;
        var parameters = AcceptManySeparated<IGenericParameterSyntax, ICommaToken>(AcceptGenericParameter);
        var closeBracketSpan = Tokens.Required<ICloseBracketToken>();
        return (parameters, TextSpan.Covering(openBracket.Span, closeBracketSpan));
    }

    private IGenericParameterSyntax? AcceptGenericParameter()
    {
        var constraint = AcceptExplicitCapabilityConstraint()
                         ?? CapabilitySetSyntax.ImplicitAliasable(Tokens.Current.Span.AtStart());
        var identifier = Tokens.AcceptToken<IIdentifierToken>();
        if (identifier is null) return null;
        var (independence, independenceSpan) = ParseIndependence();
        var (variance, varianceSpan) = ParseVariance();
        var span = TextSpan.Covering(identifier.Span, independenceSpan, varianceSpan);
        return new GenericParameterSyntax(span, constraint, identifier.Value, independence, variance);
    }

    private (TypeParameterIndependence, TextSpan) ParseIndependence()
    {
        return Tokens.Current switch
        {
            IIndependentKeywordToken _ => (TypeParameterIndependence.Independent, Tokens.Consume<IIndependentKeywordToken>()),
            IShareableKeywordToken _ => ParseSharableIndependence(),
            _ => (TypeParameterIndependence.None, Tokens.Current.Span.AtStart())
        };
    }

    private (TypeParameterIndependence, TextSpan) ParseSharableIndependence()
    {
        var shareableKeyword = Tokens.Required<IShareableKeywordToken>();
        var independentKeyword = Tokens.Required<IIndependentKeywordToken>();
        var span = TextSpan.Covering(shareableKeyword, independentKeyword);
        return (TypeParameterIndependence.SharableIndependent, span);
    }

    private (TypeParameterVariance, TextSpan) ParseVariance()
    {
        return Tokens.Current switch
        {
            IInKeywordToken _ => (TypeParameterVariance.Contravariant, Tokens.Consume<IInKeywordToken>()),
            IOutKeywordToken _ => (TypeParameterVariance.Covariant, Tokens.Consume<IOutKeywordToken>()),
            INonwritableKeywordToken _ => ParseNonwriteableVariance(),
            _ => (TypeParameterVariance.Invariant, Tokens.Current.Span.AtStart())
        };
    }

    private (TypeParameterVariance, TextSpan) ParseNonwriteableVariance()
    {
        var nonwriteableKeyword = Tokens.Required<INonwritableKeywordToken>();
        var outKeyword = Tokens.Required<IOutKeywordToken>();
        var span = TextSpan.Covering(nonwriteableKeyword, outKeyword);
        return (TypeParameterVariance.NonwritableCovariant, span);
    }

    private (IFixedList<IClassMemberDefinitionSyntax> Members, TextSpan Span) ParseClassBody()
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var members = ParseClassMemberDefinitions();
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return (members, span);
    }

    private IFixedList<IClassMemberDefinitionSyntax> ParseClassMemberDefinitions()
        => ParseMany<IClassMemberDefinitionSyntax, ICloseBraceToken>(ParseClassMemberDefinition);

    internal IClassMemberDefinitionSyntax ParseClassMemberDefinition()
    {
        var modifiers = ParseModifiers();

        switch (Tokens.Current)
        {
            case IFunctionKeywordToken _:
                return ParseClassMemberFunction(modifiers);
            case IGetKeywordToken _:
                return ParseGetterMethod(modifiers);
            case ISetKeywordToken _:
                return ParseSetterMethod(modifiers);
            case INewKeywordToken _:
                return ParseConstructor(modifiers);
            case ILetKeywordToken _:
                return ParseField(false, modifiers);
            case IVarKeywordToken _:
                return ParseField(true, modifiers);
            default:
                Tokens.UnexpectedToken();
                throw new ParseFailedException();
        }
    }
    #endregion

    #region Parse Struct Declarations
    private IStructDefinitionSyntax ParseStruct(ModifierParser modifiers)
    {
        var accessModifier = modifiers.ParseAccessModifier();
        var constModifier = modifiers.ParseConstModifier();
        var structKindModifier = modifiers.ParseStructKindModifier();
        modifiers.ParseEndOfModifiers();
        var structKeywordSpan = Tokens.Consume<IStructKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var name = identifier.Value;
        var generic = AcceptGenericParameters();
        var genericParameters = generic?.Parameters ?? FixedList.Empty<IGenericParameterSyntax>();
        var superTypes = ParseSupertypes();
        var (members, bodySpan) = ParseStructBody();
        var span = TextSpan.Covering(structKeywordSpan, identifier.Span, generic?.Span,
            TextSpan.Covering(superTypes.Select(st => st.Span)), bodySpan);
        // TODO parse nested traits
        return new StructDefinitionSyntax(span, File, accessModifier,
            constModifier, structKindModifier, identifier.Span, name, genericParameters, superTypes,
            members);
    }

    private (IFixedList<IStructMemberDefinitionSyntax> Members, TextSpan Span) ParseStructBody()
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var members = ParseStructMemberDefinitions();
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return (members, span);
    }

    private IFixedList<IStructMemberDefinitionSyntax> ParseStructMemberDefinitions()
        => ParseMany<IStructMemberDefinitionSyntax, ICloseBraceToken>(ParseStructMemberDefinition);

    internal IStructMemberDefinitionSyntax ParseStructMemberDefinition()
    {
        var modifiers = ParseModifiers();

        switch (Tokens.Current)
        {
            case IFunctionKeywordToken _:
                return ParseStructMemberFunction(modifiers);
            case IGetKeywordToken _:
                return ParseGetterMethod(modifiers);
            case ISetKeywordToken _:
                return ParseSetterMethod(modifiers);
            case IInitKeywordToken _:
                return ParseInitializer(modifiers);
            case ILetKeywordToken _:
                return ParseField(false, modifiers);
            case IVarKeywordToken _:
                return ParseField(true, modifiers);
            default:
                Tokens.UnexpectedToken();
                throw new ParseFailedException();
        }
    }
    #endregion

    #region Parse Trait Declarations
    private ITraitDefinitionSyntax ParseTrait(ModifierParser modifiers)
    {
        var accessModifier = modifiers.ParseAccessModifier();
        var constModifier = modifiers.ParseConstModifier();
        var moveModifier = modifiers.ParseMoveModifier();
        modifiers.ParseEndOfModifiers();
        var traitKeywordSpan = Tokens.Consume<ITraitKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var name = identifier.Value;
        var generic = AcceptGenericParameters();
        var genericParameters = generic?.Parameters ?? FixedList.Empty<IGenericParameterSyntax>();
        var superTypes = ParseSupertypes();
        var (members, bodySpan) = ParseTraitBody();
        var span = TextSpan.Covering(traitKeywordSpan, identifier.Span, generic?.Span,
            TextSpan.Covering(superTypes.Select(st => st.Span)), bodySpan);
        // TODO parse nested traits
        return new TraitDefinitionSyntax(span, File, accessModifier,
            constModifier, moveModifier, identifier.Span, name, genericParameters, superTypes, members);
    }

    private (IFixedList<ITraitMemberDefinitionSyntax> Members, TextSpan Span) ParseTraitBody()
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var members = ParseTraitMemberDeclarations();
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return (members, span);
    }

    private IFixedList<ITraitMemberDefinitionSyntax> ParseTraitMemberDeclarations()
        => ParseMany<ITraitMemberDefinitionSyntax, ICloseBraceToken>(ParseTraitMemberDefinition);

    internal ITraitMemberDefinitionSyntax ParseTraitMemberDefinition()
    {
        var modifiers = ParseModifiers();

        switch (Tokens.Current)
        {
            case IFunctionKeywordToken _:
                return ParseTraitMemberFunction(modifiers);
            case IGetKeywordToken _:
                return ParseGetterMethod(modifiers);
            case ISetKeywordToken _:
                return ParseSetterMethod(modifiers);
            default:
                Tokens.UnexpectedToken();
                throw new ParseFailedException();
        }
    }

    internal ITraitMemberDefinitionSyntax ParseTraitMemberFunction(ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var fn = Tokens.Consume<IFunctionKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        var parameters = ParseParameters(ParseMethodParameter);
        var @return = ParseReturn();

        var selfParameter = parameters.OfType<IMethodSelfParameterSyntax>().FirstOrDefault();
        var namedParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>())
                                        .Cast<INamedParameterSyntax>().ToFixedList();

        // if no self parameter, it is an associated function
        if (selfParameter is null)
        {
            IBodySyntax body;
            TextSpan span;
            if (Tokens.Current is IOpenBraceToken or IRightDoubleArrowToken)
            {
                body = ParseBody();
                span = TextSpan.Covering(fn, body.Span);
            }
            else
            {
                body = new BlockBodySyntax(Tokens.Current.Span.AtStart(), FixedList.Empty<IBodyStatementSyntax>());
                var semicolon = Tokens.Expect<ISemicolonToken>();
                span = TextSpan.Covering(fn, semicolon);
                Add(ParseError.AssociatedFunctionMissingBody(File, span, name));
            }

            return new AssociatedFunctionDefinitionSyntax(span, File, accessModifer, identifier.Span, name, namedParameters, @return, body);
        }

        if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        foreach (var extraSelfParameter in parameters.OfType<ISelfParameterSyntax>().Skip(1))
            Add(ParseError.ExtraSelfParameter(File, extraSelfParameter.Span));

        // It is a method that may or may not have a body
        if (Tokens.Current is IOpenBraceToken or IRightDoubleArrowToken)
        {
            var body = ParseBody();
            var span = TextSpan.Covering(fn, body.Span);
            return new StandardMethodDefinitionSyntax(span, File, accessModifer,
                identifier.Span, name, selfParameter, namedParameters, @return, body);
        }
        else
        {
            var semicolon = Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(fn, semicolon);
            return new AbstractMethodDefinitionSyntax(span, File, accessModifer,
                identifier.Span, name, selfParameter, namedParameters, @return);
        }
    }
    #endregion

    #region Parse Member Declarations
    internal IFieldDefinitionSyntax ParseField(bool mutableBinding, ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        // We should only be called when there is a binding keyword
        var binding = Tokens.Consume<IBindingToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        Tokens.Expect<IColonToken>();
        var type = ParseType();
        IExpressionSyntax? initializer = null;
        if (Tokens.Accept<IEqualsToken>())
            initializer = ParseExpression();

        var semicolon = Tokens.Expect<ISemicolonToken>();
        var span = TextSpan.Covering(binding, semicolon);
        return new FieldDefinitionSyntax(span, File, accessModifer, mutableBinding,
            identifier.Span, name, type, initializer);
    }

    internal IClassMemberDefinitionSyntax ParseClassMemberFunction(ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        var abstractModifier = modifiers.ParseAbstractModifier();
        modifiers.ParseEndOfModifiers();
        var fn = Tokens.Consume<IFunctionKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        var parameters = ParseParameters(ParseMethodParameter);
        var @return = ParseReturn();

        var selfParameter = parameters.OfType<IMethodSelfParameterSyntax>().FirstOrDefault();
        var namedParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>())
                                        .Cast<INamedParameterSyntax>().ToFixedList();

        // if no self parameter, it is an associated function
        if (selfParameter is null)
        {
            if (abstractModifier is not null)
                Add(ParseError.AbstractAssociatedFunction(File, abstractModifier.Span));
            IBodySyntax body;
            TextSpan span;
            if (Tokens.Current is IOpenBraceToken or IRightDoubleArrowToken)
            {
                body = ParseBody();
                span = TextSpan.Covering(fn, body.Span);
            }
            else
            {
                body = new BlockBodySyntax(Tokens.Current.Span.AtStart(), FixedList.Empty<IBodyStatementSyntax>());
                var semicolon = Tokens.Expect<ISemicolonToken>();
                span = TextSpan.Covering(fn, semicolon);
                Add(ParseError.AssociatedFunctionMissingBody(File, span, name));
            }

            return new AssociatedFunctionDefinitionSyntax(span, File, accessModifer, identifier.Span, name, namedParameters, @return, body);
        }

        if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        foreach (var extraSelfParameter in parameters.OfType<ISelfParameterSyntax>().Skip(1))
            Add(ParseError.ExtraSelfParameter(File, extraSelfParameter.Span));

        // It is a method that may or may not have a body
        if (Tokens.Current is IOpenBraceToken or IRightDoubleArrowToken)
        {
            if (abstractModifier is not null)
                Add(ParseError.ConcreteMethodDeclaredAbstract(File, abstractModifier.Span));
            var body = ParseBody();
            var span = TextSpan.Covering(fn, body.Span);
            return new StandardMethodDefinitionSyntax(span, File, accessModifer,
                identifier.Span, name, selfParameter, namedParameters, @return, body);
        }
        else
        {
            if (abstractModifier is null)
                Add(ParseError.AbstractMethodMissingAbstractModifier(File, identifier.Span));
            var semicolon = Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(fn, semicolon);
            return new AbstractMethodDefinitionSyntax(span, File, accessModifer,
                identifier.Span, name, selfParameter, namedParameters, @return);
        }
    }

    internal IGetterMethodDefinitionSyntax ParseGetterMethod(ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var get = Tokens.Consume<IGetKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        // Self parameter is expected to be after the current token which is expected to be `(`
        var expectedSelfParameterLocation = Tokens.Current.Span.AtEnd();
        var parameters = ParseParameters(ParseMethodParameter);
        var expectedReturnLocation = Tokens.Current.Span.AtStart();
        var @return = ParseReturn();
        var body = ParseBody();

        var selfParameter = parameters.OfType<IMethodSelfParameterSyntax>().FirstOrDefault();
        var namedParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>()).Cast<INamedParameterSyntax>()
                                        .ToFixedList();

        if (selfParameter is null)
        {
            Add(ParseError.MissingSelfParameter(File, expectedSelfParameterLocation));
            // For simplicity of downstream code, make up a fake self parameter
            var selfReferenceCapability = new CapabilitySyntax(expectedSelfParameterLocation,
                [], DeclaredCapability.Mutable);
            selfParameter = new MethodSelfParameterSyntax(expectedSelfParameterLocation, false, selfReferenceCapability);
        }
        else if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        if (namedParameters.Any())
            Add(ParseError.GetterHasParameters(File, TextSpan.Covering(namedParameters.Select(p => p.Span))!.Value));

        if (@return is null)
        {
            Add(ParseError.MissingReturn(File, expectedSelfParameterLocation));
            @return = new ReturnSyntax(expectedReturnLocation, new SpecialTypeNameSyntax(expectedReturnLocation, SpecialTypeName.Void));
        }

        var span = TextSpan.Covering(get, body.Span);
        return new GetterMethodDefinitionSyntax(span, File, accessModifer,
            identifier.Span, name, selfParameter, @return, body);
    }

    internal ISetterMethodDefinitionSyntax ParseSetterMethod(ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var set = Tokens.Consume<ISetKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        // Self parameter is expected to be after the current token which is expected to be `(`
        var expectedSelfParameterLocation = Tokens.Current.Span.AtEnd();
        var parameters = ParseParameters(ParseMethodParameter);
        var @return = ParseReturn();
        var body = ParseBody();

        var selfParameter = parameters.OfType<IMethodSelfParameterSyntax>().FirstOrDefault();
        var namedParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>()).Cast<INamedParameterSyntax>()
                                        .ToFixedList();
        if (selfParameter is null)
        {
            Add(ParseError.MissingSelfParameter(File, expectedSelfParameterLocation));
            // For simplicity of downstream code, make up a fake self parameter
            var selfReferenceCapability = new CapabilitySyntax(expectedSelfParameterLocation,
                Enumerable.Empty<ICapabilityToken>(), DeclaredCapability.Mutable);
            selfParameter = new MethodSelfParameterSyntax(expectedSelfParameterLocation, false, selfReferenceCapability);
        }
        else if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        if (!namedParameters.Any())
            Add(ParseError.SetterMissingParameter(File, selfParameter.Span.AtEnd()));
        if (namedParameters.Count > 1)
            Add(ParseError.SetterHasExtraParameters(File, TextSpan.Covering(namedParameters.Skip(1).Select(p => p.Span))!.Value));
        if (@return is not null)
            Add(ParseError.SetterHasReturn(File, @return.Span));
        var span = TextSpan.Covering(set, body.Span);
        return new SetterMethodDefinitionSyntax(span, File, accessModifer,
            identifier.Span, name, selfParameter, namedParameters.FirstOrDefault(), body);
    }

    internal IConstructorDefinitionSyntax ParseConstructor(ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var newKeywordSpan = Tokens.Consume<INewKeywordToken>();
        var identifier = Tokens.AcceptToken<IIdentifierToken>();
        var name = identifier is null ? null : (IdentifierName)identifier.Value;
        // Self parameter is expected to be after the current token which is expected to be `(`
        var expectedSelfParameterLocation = Tokens.Current.Span.AtEnd();
        var parameters = ParseParameters(ParseConstructorParameter);

        var selfParameter = parameters.OfType<IConstructorSelfParameterSyntax>().FirstOrDefault();
        var constructorParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>())
                                              .Cast<IConstructorOrInitializerParameterSyntax>()
                                        .ToFixedList();

        if (selfParameter is null)
        {
            Add(ParseError.MissingSelfParameter(File, expectedSelfParameterLocation));
            // For simplicity of downstream code, make up a fake self parameter
            var selfReferenceCapability = new CapabilitySyntax(expectedSelfParameterLocation,
                [], DeclaredCapability.Mutable);
            selfParameter = new ConstructorSelfParameterSyntax(expectedSelfParameterLocation, false, selfReferenceCapability);
        }
        else if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        foreach (var extraSelfParameter in parameters.OfType<ISelfParameterSyntax>().Skip(1))
            Add(ParseError.ExtraSelfParameter(File, extraSelfParameter.Span));

        var body = ParseBlockBody();
        // For now, just say constructors have no annotations
        var span = TextSpan.Covering(newKeywordSpan, body.Span);
        return new ConstructorDefinitionSyntax(span, File, accessModifer,
            TextSpan.Covering(newKeywordSpan, identifier?.Span), name, selfParameter, constructorParameters, body);
    }

    internal IInitializerDefinitionSyntax ParseInitializer(ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var initKeywordSpan = Tokens.Consume<IInitKeywordToken>();
        var identifier = Tokens.AcceptToken<IIdentifierToken>();
        var name = identifier is null ? null : (IdentifierName)identifier.Value;
        // Self parameter is expected to be after the current token which is expected to be `(`
        var expectedSelfParameterLocation = Tokens.Current.Span.AtEnd();
        var parameters = ParseParameters(ParseInitializerParameter);

        var selfParameter = parameters.OfType<IInitializerSelfParameterSyntax>().FirstOrDefault();
        var constructorParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>())
                                              .Cast<IConstructorOrInitializerParameterSyntax>()
                                              .ToFixedList();

        if (selfParameter is null)
        {
            Add(ParseError.MissingSelfParameter(File, expectedSelfParameterLocation));
            // For simplicity of downstream code, make up a fake self parameter
            var selfReferenceCapability = new CapabilitySyntax(expectedSelfParameterLocation,
                [], DeclaredCapability.Mutable);
            selfParameter = new InitializerSelfParameterSyntax(expectedSelfParameterLocation, false, selfReferenceCapability);
        }
        else if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        foreach (var extraSelfParameter in parameters.OfType<ISelfParameterSyntax>().Skip(1))
            Add(ParseError.ExtraSelfParameter(File, extraSelfParameter.Span));

        var body = ParseBlockBody();
        // For now, just say constructors have no annotations
        var span = TextSpan.Covering(initKeywordSpan, body.Span);
        return new InitializerDefinitionSyntax(span, File, accessModifer,
            TextSpan.Covering(initKeywordSpan, identifier?.Span), name, selfParameter, constructorParameters, body);
    }

    internal IStructMemberDefinitionSyntax ParseStructMemberFunction(ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var fn = Tokens.Consume<IFunctionKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        var parameters = ParseParameters(ParseMethodParameter);
        var @return = ParseReturn();

        var selfParameter = parameters.OfType<IMethodSelfParameterSyntax>().FirstOrDefault();
        var namedParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>())
                                        .Cast<INamedParameterSyntax>().ToFixedList();
        IBodySyntax body;
        TextSpan span;

        // if no self parameter, it is an associated function
        if (selfParameter is null)
        {
            if (Tokens.Current is IOpenBraceToken or IRightDoubleArrowToken)
            {
                body = ParseBody();
                span = TextSpan.Covering(fn, body.Span);
            }
            else
            {
                body = new BlockBodySyntax(Tokens.Current.Span.AtStart(), FixedList.Empty<IBodyStatementSyntax>());
                var semicolon = Tokens.Expect<ISemicolonToken>();
                span = TextSpan.Covering(fn, semicolon);
                Add(ParseError.AssociatedFunctionMissingBody(File, span, name));
            }

            return new AssociatedFunctionDefinitionSyntax(span, File, accessModifer, identifier.Span, name, namedParameters, @return, body);
        }

        if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        foreach (var extraSelfParameter in parameters.OfType<ISelfParameterSyntax>().Skip(1))
            Add(ParseError.ExtraSelfParameter(File, extraSelfParameter.Span));

        // It is a method that may or may not have a body
        if (Tokens.Current is IOpenBraceToken or IRightDoubleArrowToken)
        {
            body = ParseBody();
            span = TextSpan.Covering(fn, body.Span);
        }
        else
        {
            body = new BlockBodySyntax(Tokens.Current.Span.AtStart(), FixedList.Empty<IBodyStatementSyntax>());
            var semicolon = Tokens.Expect<ISemicolonToken>();
            span = TextSpan.Covering(fn, semicolon);
            Add(ParseError.StructMethodMissingBody(File, span, name));
        }

        return new StandardMethodDefinitionSyntax(span, File, accessModifer,
            identifier.Span, name, selfParameter, namedParameters, @return, body);
    }
    #endregion
}
