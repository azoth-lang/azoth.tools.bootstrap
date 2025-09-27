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
                // TODO errors on attributes
                return ParseNamespaceBlock(modifiers);
            case IClassKeywordToken _:
                return ParseClass(attributes, modifiers);
            case IStructKeywordToken _:
                return ParseStruct(attributes, modifiers);
            case IValueKeywordToken _:
                return ParseValue(attributes, modifiers);
            case ITraitKeywordToken _:
                return ParseTrait(attributes, modifiers);
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

        while (Tokens.AcceptToken<IDotToken>() is { } dot)
        {
            var nameSegment = Tokens.ExpectToken<IIdentifierToken>();
            // We need the span to cover a trailing dot
            span = TextSpan.Covering(span, dot.Span, nameSegment?.Span);
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
        var accessModifiers = modifiers.ParseAccessModifiers();
        modifiers.ParseEndOfModifiers();
        var fn = Tokens.Consume<IFunctionKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        var parameters = ParseParameters(ParseFunctionParameter);
        var @return = ParseReturn();
        var body = ParseBody();
        var span = TextSpan.Covering(accessModifiers.Span, fn, body.Span);
        return IFunctionDefinitionSyntax.Create(span, File, identifier.Span, accessModifiers,
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

    #region Parse Class Definitions
    private IClassDefinitionSyntax ParseClass(IFixedList<IAttributeSyntax> attributes, ModifierParser modifiers)
    {
        var accessModifiers = modifiers.ParseAccessModifiers();
        var abstractModifier = modifiers.ParseAbstractModifier();
        var constModifier = modifiers.ParseConstModifier();
        var dropModifier = modifiers.ParseDropModifier();
        modifiers.ParseEndOfModifiers();
        var classKeywordSpan = Tokens.Consume<IClassKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var generic = AcceptGenericParameters();
        var genericParameters = generic?.Parameters ?? FixedList.Empty<IGenericParameterSyntax>();
        var name = OrdinaryName.Create(identifier.Value, genericParameters.Count);
        INameSyntax? baseClass = null;
        if (Tokens.Accept<IColonToken>()) baseClass = ParseTypeName();
        var supertypes = ParseSupertypes();
        var (members, bodySpan) = ParseTypeBody(inTrait: false);
        var span = TextSpan.Covering(classKeywordSpan, identifier.Span, generic?.Span, baseClass?.Span,
            TextSpan.Covering(supertypes.Select(st => st.Span)), bodySpan);
        return IClassDefinitionSyntax.Create(span, File, identifier.Span, attributes, accessModifiers, constModifier,
            dropModifier, name, abstractModifier, genericParameters, baseClass, supertypes, members);
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
        // TODO capture independence in the syntax tree instead of just as an enum
        var (independence, independenceSpan) = ParseIndependence();
        var isIndependent = independence != TypeParameterIndependence.None;
        // TODO do not create implicit any in parsing
        var constraint = isIndependent ? ICapabilitySetSyntax.CreateImplicitAny(Tokens.Current.Span.AtStart())
            : (AcceptExplicitCapabilityConstraint()
               // TODO do not create implicit aliasable in parsing
               ?? ICapabilitySetSyntax.CreateImplicitAliasable(Tokens.Current.Span.AtStart()));
        var identifier = Tokens.AcceptToken<IIdentifierToken>();
        if (identifier is null) return null;

        var (variance, varianceSpan) = ParseVariance();
        var span = TextSpan.Covering(identifier.Span, independenceSpan, varianceSpan);
        return IGenericParameterSyntax.Create(span, constraint, identifier.Value, independence, variance);
    }

    private (TypeParameterIndependence, TextSpan) ParseIndependence()
    {
        var independentKeyword = Tokens.AcceptToken<IIndependentKeywordToken>();
        if (independentKeyword is null) return (TypeParameterIndependence.None, Tokens.Current.Span.AtStart());
        if (Tokens.Current is IOpenParenToken)
        {
            var openParen = Tokens.Consume<IOpenParenToken>();
            var shareableKeyword = Tokens.Expect<IShareableKeywordToken>();
            var closeParen = Tokens.Expect<ICloseParenToken>();
            var span = TextSpan.Covering(independentKeyword.Span, openParen, shareableKeyword, closeParen);
            return (TypeParameterIndependence.ShareableIndependent, span);
        }
        return (TypeParameterIndependence.Independent, independentKeyword.Span);
    }

    private (TypeParameterVariance, TextSpan) ParseVariance()
    {
        return Tokens.Current switch
        {
            IInKeywordToken _ => (TypeParameterVariance.Contravariant, Tokens.Consume<IInKeywordToken>()),
            IOutKeywordToken _ => (TypeParameterVariance.Covariant, Tokens.Consume<IOutKeywordToken>()),
            IReadonlyKeywordToken _ => ParseReadOnlyVariance(),
            _ => (TypeParameterVariance.Invariant, Tokens.Current.Span.AtStart())
        };
    }

    private (TypeParameterVariance, TextSpan) ParseReadOnlyVariance()
    {
        var readOnlyKeyword = Tokens.Required<IReadonlyKeywordToken>();
        var outKeyword = Tokens.Required<IOutKeywordToken>();
        var span = TextSpan.Covering(readOnlyKeyword, outKeyword);
        return (TypeParameterVariance.ReadOnlyCovariant, span);
    }

    private (IFixedList<IMemberDefinitionSyntax> Members, TextSpan Span) ParseTypeBody(bool inTrait)
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var members = ParseMemberDefinitions(inTrait);
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return (members, span);
    }
    #endregion

    #region Parse Struct Definitions
    private IStructDefinitionSyntax ParseStruct(IFixedList<IAttributeSyntax> attributes, ModifierParser modifiers)
    {
        var accessModifiers = modifiers.ParseAccessModifiers();
        var constModifier = modifiers.ParseConstModifier();
        var dropModifier = modifiers.ParseDropModifier();
        modifiers.ParseEndOfModifiers();
        var structKeywordSpan = Tokens.Consume<IStructKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var generic = AcceptGenericParameters();
        var genericParameters = generic?.Parameters ?? FixedList.Empty<IGenericParameterSyntax>();
        var name = OrdinaryName.Create(identifier.Value, genericParameters.Count);
        var superTypes = ParseSupertypes();
        var (members, bodySpan) = ParseTypeBody(inTrait: false);
        var span = TextSpan.Covering(structKeywordSpan, identifier.Span, generic?.Span,
            TextSpan.Covering(superTypes.Select(st => st.Span)), bodySpan);
        return IStructDefinitionSyntax.Create(span, File, identifier.Span, attributes, accessModifiers,
            constModifier, dropModifier, name, genericParameters, superTypes,
            members);
    }
    #endregion

    #region Parse Value Definitions
    private IValueDefinitionSyntax ParseValue(IFixedList<IAttributeSyntax> attributes, ModifierParser modifiers)
    {
        var accessModifiers = modifiers.ParseAccessModifiers();
        var constModifier = modifiers.ParseConstModifier();
        var dropModifier = modifiers.ParseDropModifier();
        modifiers.ParseEndOfModifiers();
        var valueKeywordSpan = Tokens.Consume<IValueKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var generic = AcceptGenericParameters();
        var genericParameters = generic?.Parameters ?? FixedList.Empty<IGenericParameterSyntax>();
        var name = OrdinaryName.Create(identifier.Value, genericParameters.Count);
        var superTypes = ParseSupertypes();
        var (members, bodySpan) = ParseTypeBody(inTrait: false);
        var span = TextSpan.Covering(valueKeywordSpan, identifier.Span, generic?.Span,
            TextSpan.Covering(superTypes.Select(st => st.Span)), bodySpan);
        return IValueDefinitionSyntax.Create(span, File, identifier.Span, attributes, accessModifiers, constModifier,
            dropModifier, name, genericParameters, superTypes, members);
    }

    #endregion

    #region Parse Trait Definitions
    private ITraitDefinitionSyntax ParseTrait(IFixedList<IAttributeSyntax> attributes, ModifierParser modifiers)
    {
        var accessModifiers = modifiers.ParseAccessModifiers();
        var constModifier = modifiers.ParseConstModifier();
        var dropModifier = modifiers.ParseDropModifier();
        modifiers.ParseEndOfModifiers();
        var traitKeywordSpan = Tokens.Consume<ITraitKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var generic = AcceptGenericParameters();
        var genericParameters = generic?.Parameters ?? FixedList.Empty<IGenericParameterSyntax>();
        var name = OrdinaryName.Create(identifier.Value, genericParameters.Count);
        var superTypes = ParseSupertypes();
        var (members, bodySpan) = ParseTypeBody(inTrait: true);
        var span = TextSpan.Covering(traitKeywordSpan, identifier.Span, generic?.Span,
            TextSpan.Covering(superTypes.Select(st => st.Span)), bodySpan);
        return ITraitDefinitionSyntax.Create(span, File, identifier.Span, attributes, accessModifiers,
            constModifier, dropModifier, name, genericParameters, superTypes, members);
    }

    #endregion

    #region Parse Member Definitions
    private IFixedList<IMemberDefinitionSyntax> ParseMemberDefinitions(bool inTrait)
        => ParseMany<IMemberDefinitionSyntax, ICloseBraceToken>(() => ParseMemberDefinition(inTrait));

    internal IMemberDefinitionSyntax ParseMemberDefinition(bool inTrait)
    {
        var attributes = ParseAttributes();
        var modifiers = ParseModifiers();

        switch (Tokens.Current)
        {
            case IFunctionKeywordToken _:
                return ParseMemberFunction(attributes, modifiers, inTrait);
            case IGetKeywordToken _:
                return ParseGetterMethod(attributes, modifiers, inTrait);
            case ISetKeywordToken _:
                return ParseSetterMethod(attributes, modifiers, inTrait);
            case IInitKeywordToken _:
                return ParseInitializer(attributes, modifiers);
            case ILetKeywordToken _:
                return ParseField(attributes, false, modifiers);
            case IVarKeywordToken _:
                return ParseField(attributes, true, modifiers);
            case IVarianceToken _:
            case ITypeKeywordToken _:
                return ParseAssociatedType(attributes, modifiers, inTrait);
            // Nested types
            case IClassKeywordToken _:
                return ParseClass(attributes, modifiers);
            case IStructKeywordToken _:
                return ParseStruct(attributes, modifiers);
            case ITraitKeywordToken _:
                return ParseTrait(attributes, modifiers);
            default:
                Tokens.UnexpectedToken();
                throw new ParseFailedException();
        }
    }

    internal IFieldDefinitionSyntax ParseField(
        IFixedList<IAttributeSyntax> attributes,
        bool mutableBinding,
        ModifierParser modifiers)
    {
        var accessModifiers = modifiers.ParseAccessModifiers();
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
        return IFieldDefinitionSyntax.Create(span, File, identifier.Span, attributes, accessModifiers, mutableBinding,
             name, type, initializer);
    }

    internal IGetterMethodDefinitionSyntax ParseGetterMethod(
        IFixedList<IAttributeSyntax> attributes,
        ModifierParser modifiers,
        bool inTrait)
    {
        var accessModifiers = modifiers.ParseAccessModifiers();
        // TODO move checking for abstract modifier rules to semantic tree
        var abstractModifier = modifiers.ParseAbstractModifier();
        modifiers.ParseEndOfModifiers();
        var get = Tokens.Consume<IGetKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        if (name == "get" || name == "set")
            Add(ParseError.InvalidGetterName(File, identifier));
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
            var span = TextSpan.Covering(accessModifiers.Span, abstractModifier?.Span, get, body.Span);
            return IGetterMethodDefinitionSyntax.Create(span, File, identifier.Span, attributes,
                accessModifiers, abstractModifier, name, selfParameter, @return, body);
        }
        else
        {
            if (!inTrait && abstractModifier is null)
                Add(ParseError.AbstractMethodMissingAbstractModifier(File, identifier.Span));
            else if (abstractModifier is not null && inTrait)
                Add(ParseError.AbstractModifierInTrait(File, abstractModifier.Span));
            var semicolon = Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(accessModifiers.Span, abstractModifier?.Span, get, semicolon);
            return IGetterMethodDefinitionSyntax.Create(span, File, identifier.Span, attributes,
                accessModifiers, abstractModifier, name, selfParameter, @return, null);
        }
    }

    internal ISetterMethodDefinitionSyntax ParseSetterMethod(
        IFixedList<IAttributeSyntax> attributes,
        ModifierParser modifiers,
        bool inTrait)
    {
        var accessModifiers = modifiers.ParseAccessModifiers();
        // TODO move checking for abstract modifier rules to semantic tree
        var abstractModifier = modifiers.ParseAbstractModifier();
        modifiers.ParseEndOfModifiers();
        var set = Tokens.Consume<ISetKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;
        if (name == "get" || name == "set")
            Add(ParseError.InvalidSetterName(File, identifier));
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
            span = TextSpan.Covering(accessModifiers.Span, abstractModifier?.Span, set, body.Span);
        }
        else
        {
            if (!inTrait && abstractModifier is null)
                Add(ParseError.AbstractMethodMissingAbstractModifier(File, identifier.Span));
            else if (abstractModifier is not null && inTrait)
                Add(ParseError.AbstractModifierInTrait(File, abstractModifier.Span));
            body = null;
            var semicolon = Tokens.Expect<ISemicolonToken>();
            span = TextSpan.Covering(accessModifiers.Span, abstractModifier?.Span, set, semicolon);
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
        return ISetterMethodDefinitionSyntax.Create(span, File, identifier.Span, attributes,
            accessModifiers, abstractModifier, name, selfParameter, namedParameters, body);
    }

    internal IInitializerDefinitionSyntax ParseInitializer(
        IFixedList<IAttributeSyntax> attributes,
        ModifierParser modifiers)
    {
        var accessModifiers = modifiers.ParseAccessModifiers();
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
        return IInitializerDefinitionSyntax.Create(span, File, nameSpan, attributes, accessModifiers,
            name, selfParameter, constructorParameters, body);
    }

    internal IMemberDefinitionSyntax ParseMemberFunction(
        IFixedList<IAttributeSyntax> attributes,
        ModifierParser modifiers,
        bool inTrait)
    {
        var accessModifiers = modifiers.ParseAccessModifiers();
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

            return IAssociatedFunctionDefinitionSyntax.Create(span, File, identifier.Span, attributes,
                accessModifiers, abstractModifier, name, namedParameters, @return, body);
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

        return IOrdinaryMethodDefinitionSyntax.Create(span, File, identifier.Span, attributes,
            accessModifiers, abstractModifier, name, selfParameter, namedParameters, @return, body);
    }

    internal IAssociatedTypeDefinitionSyntax ParseAssociatedType(
        IFixedList<IAttributeSyntax> attributes,
        ModifierParser modifiers,
        bool inTrait)
    {
        var accessModifiers = modifiers.ParseAccessModifiers();
        // TODO move checking for abstract modifier rules to semantic tree
        var abstractModifier = modifiers.ParseAbstractModifier();
        modifiers.ParseEndOfModifiers();
        var variance = Tokens.AcceptToken<IVarianceToken>();
        var typeKeyword = Tokens.ConsumeToken<ITypeKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        IdentifierName name = identifier.Value;

        var equalsOperator = Tokens.AcceptToken<IEqualsToken>();
        ITypeSyntax? initializer = null;
        if (equalsOperator is not null)
        {
            if (abstractModifier is not null)
                Add(ParseError.ConcreteAssociatedTypeDeclaredAbstract(File, abstractModifier.Span));

            initializer = ParseType();
        }
        else
        {
            if (abstractModifier is null && !inTrait)
                Add(ParseError.AbstractAssociatedTypeMissingAbstractModifier(File, identifier.Span));
            else if (abstractModifier is not null && inTrait)
                Add(ParseError.AbstractModifierInTrait(File, abstractModifier.Span));
        }

        var semicolon = Tokens.Expect<ISemicolonToken>();
        var span = TextSpan.Covering(accessModifiers.Span, variance?.Span, typeKeyword.Span, semicolon);
        return IAssociatedTypeDefinitionSyntax.Create(span, File, identifier.Span, attributes,
            accessModifiers, abstractModifier, variance, typeKeyword, name, equalsOperator, initializer);
    }
    #endregion
}
