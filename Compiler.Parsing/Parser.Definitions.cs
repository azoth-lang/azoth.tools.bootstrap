using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    public IFixedList<INamespaceBlockMemberDefinitionSyntax> ParseNamespaceBlockMemberDefinitions<T>()
        where T : IToken
    {
        var declarations = new List<INamespaceBlockMemberDefinitionSyntax>();
        // Stop at end of file or some token that contains these declarations
        while (!Tokens.AtEnd<T>())
            try
            {
                declarations.Add(ParseNamespaceBlockMemberDefinition());
            }
            catch (ParseFailedException)
            {
                // Ignore: we would have consumed something before failing, try to get the next declaration
            }

        return declarations.ToFixedList();
    }

    public INamespaceBlockMemberDefinitionSyntax ParseNamespaceBlockMemberDefinition()
    {
        var attributes = ParseAttributes();
        var modifiers = ParseModifiers();

        switch (Tokens.Current)
        {
            case INamespaceKeywordToken _:
                return ParseNamespaceBlock(modifiers);
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

    private IFixedList<INameSyntax> ParseSupertypes()
        => Tokens.Accept<ILessThanColonToken>()
            ? ParseTypeNames()
            : FixedList.Empty<IOrdinaryNameSyntax>();

    #region Parse Namespaces
    internal INamespaceBlockDefinitionSyntax ParseNamespaceBlock(ModifierParser modifiers)
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
        var importDirectives = ParseImportDirectives();
        var declarations = ParseNamespaceBlockMemberDefinitions<ICloseBraceToken>();
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(ns, closeBrace);
        var isGlobalQualified = globalQualifier is not null;
        return INamespaceBlockDefinitionSyntax.Create(span, File, name.Segments.LastOrDefault(), nameSpan, isGlobalQualified, name,
            importDirectives, declarations);
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
        return IFunctionDefinitionSyntax.Create(span, File, identifier.Span, accessModifer,
            attributes, name, parameters, @return, body);
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
        return IExpressionBodySyntax.Create(resultStatement.Span, resultStatement);
    }

    private IBlockBodySyntax ParseBlockBody()
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var statements = ParseMany<IStatementSyntax, ICloseBraceToken>(ParseStatement);
        foreach (var resultStatement in statements.OfType<IResultStatementSyntax>())
            Add(ParseError.ResultStatementInBody(File, resultStatement.Span));
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return IBlockBodySyntax.Create(span, statements.OfType<IBodyStatementSyntax>().ToFixedList());
    }

    private IReturnSyntax? ParseReturn()
    {
        var rightArrow = Tokens.AcceptToken<IRightArrowToken>();
        if (rightArrow is null)
            return null;
        var type = ParseType();
        var span = TextSpan.Covering(rightArrow.Span, type.Span);
        return IReturnSyntax.Create(span, type);
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
        var generic = AcceptGenericParameters();
        var genericParameters = generic?.Parameters ?? FixedList.Empty<IGenericParameterSyntax>();
        var name = OrdinaryName.Create(identifier.Value, genericParameters.Count);
        INameSyntax? baseClass = null;
        if (Tokens.Accept<IColonToken>()) baseClass = ParseTypeName();
        var supertypes = ParseSupertypes();
        var (members, bodySpan) = ParseClassBody();
        var span = TextSpan.Covering(classKeywordSpan, identifier.Span, generic?.Span, baseClass?.Span,
            TextSpan.Covering(supertypes.Select(st => st.Span)), bodySpan);
        // TODO parse nested traits
        return IClassDefinitionSyntax.Create(span, File, identifier.Span, accessModifier, constModifier,
            moveModifier, name, abstractModifier, genericParameters, baseClass, supertypes, members);
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
                         // TODO do not do this in parsing
                         ?? ICapabilitySetSyntax.CreateImplicitAliasable(Tokens.Current.Span.AtStart());
        var identifier = Tokens.AcceptToken<IIdentifierToken>();
        if (identifier is null) return null;
        var (independence, independenceSpan) = ParseIndependence();
        var (variance, varianceSpan) = ParseVariance();
        var span = TextSpan.Covering(identifier.Span, independenceSpan, varianceSpan);
        return IGenericParameterSyntax.Create(span, constraint, identifier.Value, independence, variance);
    }

    private (TypeParameterIndependence, TextSpan) ParseIndependence()
    {
        return Tokens.Current switch
        {
            IIndependentKeywordToken _ => (TypeParameterIndependence.Independent, Tokens.Consume<IIndependentKeywordToken>()),
            IShareableKeywordToken _ => ParseShareableIndependence(),
            _ => (TypeParameterIndependence.None, Tokens.Current.Span.AtStart())
        };
    }

    private (TypeParameterIndependence, TextSpan) ParseShareableIndependence()
    {
        var shareableKeyword = Tokens.Required<IShareableKeywordToken>();
        var independentKeyword = Tokens.Required<IIndependentKeywordToken>();
        var span = TextSpan.Covering(shareableKeyword, independentKeyword);
        return (TypeParameterIndependence.ShareableIndependent, span);
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

    private (IFixedList<IMemberDefinitionSyntax> Members, TextSpan Span) ParseClassBody()
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var members = ParseMemberDefinitions(inTrait: false);
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return (members, span);
    }
    #endregion

    #region Parse Struct Declarations
    private IStructDefinitionSyntax ParseStruct(ModifierParser modifiers)
    {
        var accessModifier = modifiers.ParseAccessModifier();
        var constModifier = modifiers.ParseConstModifier();
        var moveModifier = modifiers.ParseMoveModifier();
        modifiers.ParseEndOfModifiers();
        var structKeywordSpan = Tokens.Consume<IStructKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var generic = AcceptGenericParameters();
        var genericParameters = generic?.Parameters ?? FixedList.Empty<IGenericParameterSyntax>();
        var name = OrdinaryName.Create(identifier.Value, genericParameters.Count);
        var superTypes = ParseSupertypes();
        var (members, bodySpan) = ParseStructBody();
        var span = TextSpan.Covering(structKeywordSpan, identifier.Span, generic?.Span,
            TextSpan.Covering(superTypes.Select(st => st.Span)), bodySpan);
        // TODO parse nested traits
        return IStructDefinitionSyntax.Create(span, File, identifier.Span, accessModifier,
            constModifier, moveModifier, name, genericParameters, superTypes,
            members);
    }

    private (IFixedList<IMemberDefinitionSyntax> Members, TextSpan Span) ParseStructBody()
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var members = ParseMemberDefinitions(inTrait: false);
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return (members, span);
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
        var generic = AcceptGenericParameters();
        var genericParameters = generic?.Parameters ?? FixedList.Empty<IGenericParameterSyntax>();
        var name = OrdinaryName.Create(identifier.Value, genericParameters.Count);
        var superTypes = ParseSupertypes();
        var (members, bodySpan) = ParseTraitBody();
        var span = TextSpan.Covering(traitKeywordSpan, identifier.Span, generic?.Span,
            TextSpan.Covering(superTypes.Select(st => st.Span)), bodySpan);
        // TODO parse nested traits
        return ITraitDefinitionSyntax.Create(span, File, identifier.Span, accessModifier,
            constModifier, moveModifier, name, genericParameters, superTypes, members);
    }

    private (IFixedList<IMemberDefinitionSyntax> Members, TextSpan Span) ParseTraitBody()
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var members = ParseMemberDefinitions(inTrait: true);
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return (members, span);
    }
    #endregion

    #region Parse Member Declarations
    private IFixedList<IMemberDefinitionSyntax> ParseMemberDefinitions(bool inTrait)
        => ParseMany<IMemberDefinitionSyntax, ICloseBraceToken>(() => ParseMemberDefinition(inTrait));

    internal IMemberDefinitionSyntax ParseMemberDefinition(bool inTrait)
    {
        var modifiers = ParseModifiers();

        switch (Tokens.Current)
        {
            case IFunctionKeywordToken _:
                return ParseMemberFunction(modifiers, inTrait);
            case IGetKeywordToken _:
                return ParseGetterMethod(modifiers, inTrait);
            case ISetKeywordToken _:
                return ParseSetterMethod(modifiers, inTrait);
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
        return IFieldDefinitionSyntax.Create(span, File, identifier.Span, accessModifer, mutableBinding,
             name, type, initializer);
    }

    internal IGetterMethodDefinitionSyntax ParseGetterMethod(ModifierParser modifiers, bool inTrait)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        // TODO move checking for abstract modifier rules to semantic tree
        var abstractModifier = modifiers.ParseAbstractModifier();
        modifiers.ParseEndOfModifiers();
        var get = Tokens.Consume<IGetKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        // Self parameter is expected to be after the current token which is expected to be `(`
        var expectedSelfParameterLocation = Tokens.Current.Span.AtEnd();
        var parameters = ParseParameters(ParseMethodParameter);
        var expectedReturnLocation = Tokens.Current.Span.AtStart();
        var @return = ParseReturn();

        var selfParameter = parameters.OfType<IMethodSelfParameterSyntax>().FirstOrDefault();
        var namedParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>()).Cast<INamedParameterSyntax>()
                                        .ToFixedList();

        if (selfParameter is null)
        {
            Add(ParseError.MissingSelfParameter(File, expectedSelfParameterLocation));
            // For simplicity of downstream code, make up a fake self parameter
            var selfReferenceCapability = ICapabilitySyntax.Create(expectedSelfParameterLocation,
                [], DeclaredCapability.Mutable);
            selfParameter = IMethodSelfParameterSyntax.Create(expectedSelfParameterLocation, false, selfReferenceCapability);
        }
        else if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        if (namedParameters.Any())
            Add(ParseError.GetterHasParameters(File, TextSpan.Covering(namedParameters.Select(p => p.Span))!.Value));

        if (@return is null)
        {
            Add(ParseError.MissingReturn(File, expectedSelfParameterLocation));
            @return = IReturnSyntax.Create(expectedReturnLocation, IBuiltInTypeNameSyntax.Create(expectedReturnLocation, BuiltInTypeName.Void));
        }

        // It may or may not have a body
        if (Tokens.Current is IOpenBraceToken or IRightDoubleArrowToken)
        {
            if (abstractModifier is not null)
                Add(ParseError.ConcreteMethodDeclaredAbstract(File, abstractModifier.Span));
            var body = ParseBody();
            var span = TextSpan.Covering(accessModifer?.Span, abstractModifier?.Span, get, body.Span);
            return IGetterMethodDefinitionSyntax.Create(span, File, identifier.Span, accessModifer,
                abstractModifier, name, selfParameter, @return, body);
        }
        else
        {
            // TODO if inTrait, cannot use abstract modifier
            if (!inTrait && abstractModifier is null)
                Add(ParseError.AbstractMethodMissingAbstractModifier(File, identifier.Span));
            var semicolon = Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(accessModifer?.Span, abstractModifier?.Span, get, semicolon);
            return IGetterMethodDefinitionSyntax.Create(span, File, identifier.Span, accessModifer,
                abstractModifier, name, selfParameter, @return, null);
        }
    }

    internal ISetterMethodDefinitionSyntax ParseSetterMethod(ModifierParser modifiers, bool inTrait)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        // TODO move checking for abstract modifier rules to semantic tree
        var abstractModifier = modifiers.ParseAbstractModifier();
        modifiers.ParseEndOfModifiers();
        var set = Tokens.Consume<ISetKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        // Self parameter is expected to be after the current token which is expected to be `(`
        var expectedSelfParameterLocation = Tokens.Current.Span.AtEnd();
        var parameters = ParseParameters(ParseMethodParameter);
        var @return = ParseReturn();
        IBodySyntax? body;
        TextSpan span;
        // It may or may not have a body
        if (Tokens.Current is IOpenBraceToken or IRightDoubleArrowToken)
        {
            if (abstractModifier is not null)
                Add(ParseError.ConcreteMethodDeclaredAbstract(File, abstractModifier.Span));
            body = ParseBody();
            span = TextSpan.Covering(accessModifer?.Span, abstractModifier?.Span, set, body.Span);
        }
        else
        {
            // TODO if inTrait, cannot use abstract modifier
            if (!inTrait && abstractModifier is null)
                Add(ParseError.AbstractMethodMissingAbstractModifier(File, identifier.Span));
            body = null;
            var semicolon = Tokens.Expect<ISemicolonToken>();
            span = TextSpan.Covering(accessModifer?.Span, abstractModifier?.Span, set, semicolon);
        }

        var selfParameter = parameters.OfType<IMethodSelfParameterSyntax>().FirstOrDefault();
        var namedParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>()).Cast<INamedParameterSyntax>()
                                        .ToFixedList();
        if (selfParameter is null)
        {
            Add(ParseError.MissingSelfParameter(File, expectedSelfParameterLocation));
            // For simplicity of downstream code, make up a fake self parameter
            var selfReferenceCapability = ICapabilitySyntax.Create(expectedSelfParameterLocation,
                [], DeclaredCapability.Mutable);
            selfParameter = IMethodSelfParameterSyntax.Create(expectedSelfParameterLocation, false, selfReferenceCapability);
        }
        else if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        if (!namedParameters.Any())
            Add(ParseError.SetterMissingParameter(File, selfParameter.Span.AtEnd()));
        if (namedParameters.Count > 1)
            Add(ParseError.SetterHasExtraParameters(File, TextSpan.Covering(namedParameters.Skip(1).Select(p => p.Span))!.Value));
        if (@return is not null)
            Add(ParseError.SetterHasReturn(File, @return.Span));
        return ISetterMethodDefinitionSyntax.Create(span, File, identifier.Span, accessModifer,
            abstractModifier, name, selfParameter, namedParameters, body);
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
                                              .Cast<IInitializerParameterSyntax>()
                                              .ToFixedList();

        if (selfParameter is null)
        {
            Add(ParseError.MissingSelfParameter(File, expectedSelfParameterLocation));
            // For simplicity of downstream code, make up a fake self parameter
            var selfReferenceCapability = ICapabilitySyntax.Create(expectedSelfParameterLocation,
                [], DeclaredCapability.Mutable);
            selfParameter = IInitializerSelfParameterSyntax.Create(expectedSelfParameterLocation, false, selfReferenceCapability);
        }
        else if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        foreach (var extraSelfParameter in parameters.OfType<ISelfParameterSyntax>().Skip(1))
            Add(ParseError.ExtraSelfParameter(File, extraSelfParameter.Span));

        var body = ParseBlockBody();
        // For now, just say constructors have no annotations
        var span = TextSpan.Covering(initKeywordSpan, body.Span);
        var nameSpan = TextSpan.Covering(initKeywordSpan, identifier?.Span);
        return IInitializerDefinitionSyntax.Create(span, File, nameSpan, accessModifer,
            name, selfParameter, constructorParameters, body);
    }

    internal IMemberDefinitionSyntax ParseMemberFunction(ModifierParser modifiers, bool inTrait)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        // TODO move checking for abstract modifier rules to semantic tree
        var abstractModifier = modifiers.ParseAbstractModifier();
        modifiers.ParseEndOfModifiers();
        var fn = Tokens.Consume<IFunctionKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        var parameters = ParseParameters(ParseMethodParameter);
        var @return = ParseReturn();

        var selfParameter = parameters.OfType<IMethodSelfParameterSyntax>().FirstOrDefault();
        var namedParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>()).Cast<INamedParameterSyntax>()
                                        .ToFixedList();

        IBodySyntax? body;
        TextSpan span;

        // if no self parameter, it is an associated function
        if (selfParameter is null)
        {
            if (!inTrait && abstractModifier is not null)
                Add(ParseError.AbstractAssociatedFunction(File, abstractModifier.Span));
            if (Tokens.Current is IOpenBraceToken or IRightDoubleArrowToken)
            {
                body = ParseBody();
                span = TextSpan.Covering(fn, body.Span);
            }
            else
            {
                body = IBlockBodySyntax.Create(Tokens.Current.Span.AtStart(), FixedList.Empty<IBodyStatementSyntax>());
                var semicolon = Tokens.Expect<ISemicolonToken>();
                span = TextSpan.Covering(fn, semicolon);
                Add(ParseError.AssociatedFunctionMissingBody(File, span, name));
            }

            return IAssociatedFunctionDefinitionSyntax.Create(span, File, identifier.Span,
                accessModifer, abstractModifier, name, namedParameters, @return, body);
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
            body = ParseBody();
            span = TextSpan.Covering(fn, body.Span);
        }
        else
        {
            if (!inTrait && abstractModifier is null)
                Add(ParseError.AbstractMethodMissingAbstractModifier(File, identifier.Span));
            body = null;
            var semicolon = Tokens.Expect<ISemicolonToken>();
            span = TextSpan.Covering(fn, semicolon);
        }

        return IOrdinaryMethodDefinitionSyntax.Create(span, File, identifier.Span, accessModifer,
            abstractModifier, name, selfParameter, namedParameters, @return, body);
    }
    #endregion
}
