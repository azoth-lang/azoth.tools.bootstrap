using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    public IFixedList<INonMemberDeclarationSyntax> ParseNonMemberDeclarations<T>()
        where T : IToken
    {
        var declarations = new List<INonMemberDeclarationSyntax>();
        // Stop at end of file or some token that contains these declarations
        while (!Tokens.AtEnd<T>())
            try
            {
                declarations.Add(ParseNonMemberDeclaration());
            }
            catch (ParseFailedException)
            {
                // Ignore: we would have consumed something before failing, try to get the next declaration
            }

        return declarations.ToFixedList();
    }

    public INonMemberDeclarationSyntax ParseNonMemberDeclaration()
    {
        var attributes = ParseAttributes();
        var modifiers = ParseModifiers();

        switch (Tokens.Current)
        {
            case INamespaceKeywordToken _:
                return ParseNamespaceDeclaration(modifiers);
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

    private IFixedList<ITypeNameSyntax> ParseSupertypes()
        => Tokens.Accept<ILessThanColonToken>() ? ParseTypeNames() : FixedList.Empty<ITypeNameSyntax>();

    #region Parse Namespaces
    internal NamespaceDeclarationSyntax ParseNamespaceDeclaration(ModifierParser modifiers)
    {
        modifiers.ParseEndOfModifiers();
        var ns = Tokens.Consume<INamespaceKeywordToken>();
        var globalQualifier = Tokens.AcceptToken<IColonColonDotToken>();
        var (name, nameSpan) = ParseNamespaceName();
        nameSpan = TextSpan.Covering(nameSpan, globalQualifier?.Span);
        Tokens.Expect<IOpenBraceToken>();
        var bodyParser = NamespaceBodyParser(name);
        var usingDirectives = bodyParser.ParseUsingDirectives();
        var declarations = bodyParser.ParseNonMemberDeclarations<ICloseBraceToken>();
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(ns, closeBrace);
        return new NamespaceDeclarationSyntax(ContainingNamespace, span, File, globalQualifier is not null, name, nameSpan, usingDirectives, declarations);
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
    internal IFunctionDeclarationSyntax ParseFunction(
        IFixedList<IAttributeSyntax> attributes,
        ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var fn = Tokens.Consume<IFunctionKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        SimpleName name = identifier.Value;
        var bodyParser = BodyParser();
        var parameters = bodyParser.ParseParameters(bodyParser.ParseFunctionParameter);
        var @return = ParseReturn();
        var body = bodyParser.ParseBody();
        var span = TextSpan.Covering(accessModifer?.Span, fn, body.Span);
        return new FunctionDeclarationSyntax(ContainingNamespace, span, File, attributes,
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
    private IClassDeclarationSyntax ParseClass(
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
        ITypeNameSyntax? baseClass = null;
        if (Tokens.Accept<IColonToken>()) baseClass = ParseTypeName();
        var superTypes = ParseSupertypes();
        var headerSpan = TextSpan.Covering(classKeywordSpan, identifier.Span, generic?.Span, baseClass?.Span,
            TextSpan.Covering(superTypes.Select(st => st.Span)));
        var bodyParser = BodyParser();
        return new ClassDeclarationSyntax(ContainingNamespace, headerSpan, File, accessModifier,
            abstractModifier, constModifier, moveModifier, identifier.Span, name, genericParameters,
            baseClass, superTypes, bodyParser.ParseClassBody);
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

    private (ParameterIndependence, TextSpan) ParseIndependence()
    {
        return Tokens.Current switch
        {
            IIndependentKeywordToken _ => (ParameterIndependence.Independent, Tokens.Consume<IIndependentKeywordToken>()),
            IShareableKeywordToken _ => ParseSharableIndependence(),
            _ => (ParameterIndependence.None, Tokens.Current.Span.AtStart())
        };
    }

    private (ParameterIndependence, TextSpan) ParseSharableIndependence()
    {
        var shareableKeyword = Tokens.Required<IShareableKeywordToken>();
        var independentKeyword = Tokens.Required<IIndependentKeywordToken>();
        var span = TextSpan.Covering(shareableKeyword, independentKeyword);
        return (ParameterIndependence.SharableIndependent, span);
    }

    private (ParameterVariance, TextSpan) ParseVariance()
    {
        return Tokens.Current switch
        {
            IInKeywordToken _ => (ParameterVariance.Contravariant, Tokens.Consume<IInKeywordToken>()),
            IOutKeywordToken _ => (ParameterVariance.Covariant, Tokens.Consume<IOutKeywordToken>()),
            INonwritableKeywordToken _ => ParseNonwriteableVariance(),
            _ => (ParameterVariance.Invariant, Tokens.Current.Span.AtStart())
        };
    }

    private (ParameterVariance, TextSpan) ParseNonwriteableVariance()
    {
        var nonwriteableKeyword = Tokens.Required<INonwritableKeywordToken>();
        var outKeyword = Tokens.Required<IOutKeywordToken>();
        var span = TextSpan.Covering(nonwriteableKeyword, outKeyword);
        return (ParameterVariance.NonwritableCovariant, span);
    }

    private (IFixedList<IClassMemberDeclarationSyntax> Members, TextSpan Span) ParseClassBody(IClassDeclarationSyntax declaringType)
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var members = ParseClassMemberDeclarations(declaringType);
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return (members, span);
    }

    private IFixedList<IClassMemberDeclarationSyntax> ParseClassMemberDeclarations(IClassDeclarationSyntax declaringType)
        => ParseMany<IClassMemberDeclarationSyntax, ICloseBraceToken>(() => ParseClassMemberDeclaration(declaringType));

    internal IClassMemberDeclarationSyntax ParseClassMemberDeclaration(IClassDeclarationSyntax classDeclaration)
    {
        var modifiers = ParseModifiers();

        switch (Tokens.Current)
        {
            case IFunctionKeywordToken _:
                return ParseClassMemberFunction(classDeclaration, modifiers);
            case INewKeywordToken _:
                return ParseConstructor(classDeclaration, modifiers);
            case ILetKeywordToken _:
                return ParseField(classDeclaration, false, modifiers);
            case IVarKeywordToken _:
                return ParseField(classDeclaration, true, modifiers);
            default:
                Tokens.UnexpectedToken();
                throw new ParseFailedException();
        }
    }
    #endregion

    #region Parse Struct Declarations
    private IStructDeclarationSyntax ParseStruct(ModifierParser modifiers)
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
        var headerSpan = TextSpan.Covering(structKeywordSpan, identifier.Span, generic?.Span,
            TextSpan.Covering(superTypes.Select(st => st.Span)));
        var bodyParser = BodyParser();
        return new StructDeclarationSyntax(ContainingNamespace, headerSpan, File, accessModifier,
            constModifier, structKindModifier, identifier.Span, name, genericParameters, superTypes,
            bodyParser.ParseStructBody);
    }

    private (IFixedList<IStructMemberDeclarationSyntax> Members, TextSpan Span) ParseStructBody(
        IStructDeclarationSyntax declaringType)
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var members = ParseStructMemberDeclarations(declaringType);
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return (members, span);
    }

    private IFixedList<IStructMemberDeclarationSyntax> ParseStructMemberDeclarations(IStructDeclarationSyntax declaringType)
        => ParseMany<IStructMemberDeclarationSyntax, ICloseBraceToken>(() => ParseStructMemberDeclaration(declaringType));

    internal IStructMemberDeclarationSyntax ParseStructMemberDeclaration(IStructDeclarationSyntax structDeclaration)
    {
        var modifiers = ParseModifiers();

        switch (Tokens.Current)
        {
            case IFunctionKeywordToken _:
                return ParseStructMemberFunction(structDeclaration, modifiers);
            case IInitKeywordToken _:
                return ParseInitializer(structDeclaration, modifiers);
            case ILetKeywordToken _:
                return ParseField(structDeclaration, false, modifiers);
            case IVarKeywordToken _:
                return ParseField(structDeclaration, true, modifiers);
            default:
                Tokens.UnexpectedToken();
                throw new ParseFailedException();
        }
    }
    #endregion

    #region Parse Trait Declarations
    private ITraitDeclarationSyntax ParseTrait(ModifierParser modifiers)
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
        var headerSpan = TextSpan.Covering(traitKeywordSpan, identifier.Span, generic?.Span,
            TextSpan.Covering(superTypes.Select(st => st.Span)));
        var bodyParser = BodyParser();
        return new TraitDeclarationSyntax(ContainingNamespace, headerSpan, File, accessModifier,
            constModifier, moveModifier, identifier.Span, name, genericParameters, superTypes, bodyParser.ParseTraitBody);
    }

    private (IFixedList<ITraitMemberDeclarationSyntax> Members, TextSpan Span) ParseTraitBody(
        ITraitDeclarationSyntax declaringType)
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var members = ParseTraitMemberDeclarations(declaringType);
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return (members, span);
    }

    private IFixedList<ITraitMemberDeclarationSyntax> ParseTraitMemberDeclarations(ITraitDeclarationSyntax declaringType)
        => ParseMany<ITraitMemberDeclarationSyntax, ICloseBraceToken>(() => ParseTraitMemberDeclaration(declaringType));

    internal ITraitMemberDeclarationSyntax ParseTraitMemberDeclaration(ITraitDeclarationSyntax traitDeclaration)
    {
        var modifiers = ParseModifiers();

        switch (Tokens.Current)
        {
            case IFunctionKeywordToken _:
                return ParseTraitMemberFunction(traitDeclaration, modifiers);
            default:
                Tokens.UnexpectedToken();
                throw new ParseFailedException();
        }
    }

    internal ITraitMemberDeclarationSyntax ParseTraitMemberFunction(
        ITraitDeclarationSyntax declaringType,
        ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var fn = Tokens.Consume<IFunctionKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        SimpleName name = identifier.Value;
        var bodyParser = BodyParser();
        var parameters = bodyParser.ParseParameters(bodyParser.ParseMethodParameter);
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
                body = bodyParser.ParseBody();
                span = TextSpan.Covering(fn, body.Span);
            }
            else
            {
                body = new BlockBodySyntax(Tokens.Current.Span.AtStart(), FixedList.Empty<IBodyStatementSyntax>());
                var semicolon = bodyParser.Tokens.Expect<ISemicolonToken>();
                span = TextSpan.Covering(fn, semicolon);
                Add(ParseError.AssociatedFunctionMissingBody(File, span, name));
            }

            return new AssociatedFunctionDeclarationSyntax(declaringType, span, File, accessModifer, identifier.Span, name, namedParameters, @return, body);
        }

        if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        foreach (var extraSelfParameter in parameters.OfType<ISelfParameterSyntax>().Skip(1))
            Add(ParseError.ExtraSelfParameter(File, extraSelfParameter.Span));

        // It is a method that may or may not have a body
        if (Tokens.Current is IOpenBraceToken or IRightDoubleArrowToken)
        {
            var body = bodyParser.ParseBody();
            var span = TextSpan.Covering(fn, body.Span);
            return new ConcreteMethodDeclarationSyntax(declaringType, span, File, accessModifer,
                identifier.Span, name, selfParameter, namedParameters, @return, body);
        }
        else
        {
            var semicolon = bodyParser.Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(fn, semicolon);
            return new AbstractMethodDeclarationSyntax(declaringType, span, File, accessModifer,
                identifier.Span, name, selfParameter, namedParameters, @return);
        }
    }
    #endregion

    #region Parse Member Declarations
    internal FieldDeclarationSyntax ParseField(
        IClassOrStructDeclarationSyntax declaringType,
        bool mutableBinding,
        ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        // We should only be called when there is a binding keyword
        var binding = Tokens.Consume<IBindingToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        SimpleName name = identifier.Value;
        Tokens.Expect<IColonToken>();
        var type = ParseType();
        IExpressionSyntax? initializer = null;
        if (Tokens.Accept<IEqualsToken>())
            initializer = ParseExpression();

        var semicolon = Tokens.Expect<ISemicolonToken>();
        var span = TextSpan.Covering(binding, semicolon);
        return new FieldDeclarationSyntax(declaringType, span, File, accessModifer, mutableBinding,
            identifier.Span, name, type, initializer);
    }

    internal IClassMemberDeclarationSyntax ParseClassMemberFunction(
        IClassDeclarationSyntax declaringType,
        ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        var abstractModifier = modifiers.ParseAbstractModifier();
        modifiers.ParseEndOfModifiers();
        var fn = Tokens.Consume<IFunctionKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        SimpleName name = identifier.Value;
        var bodyParser = BodyParser();
        var parameters = bodyParser.ParseParameters(bodyParser.ParseMethodParameter);
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
                body = bodyParser.ParseBody();
                span = TextSpan.Covering(fn, body.Span);
            }
            else
            {
                body = new BlockBodySyntax(Tokens.Current.Span.AtStart(), FixedList.Empty<IBodyStatementSyntax>());
                var semicolon = bodyParser.Tokens.Expect<ISemicolonToken>();
                span = TextSpan.Covering(fn, semicolon);
                Add(ParseError.AssociatedFunctionMissingBody(File, span, name));
            }

            return new AssociatedFunctionDeclarationSyntax(declaringType, span, File, accessModifer, identifier.Span, name, namedParameters, @return, body);
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
            var body = bodyParser.ParseBody();
            var span = TextSpan.Covering(fn, body.Span);
            return new ConcreteMethodDeclarationSyntax(declaringType, span, File, accessModifer,
                identifier.Span, name, selfParameter, namedParameters, @return, body);
        }
        else
        {
            if (abstractModifier is null)
                Add(ParseError.AbstractMethodMissingAbstractModifier(File, identifier.Span));
            var semicolon = bodyParser.Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(fn, semicolon);
            return new AbstractMethodDeclarationSyntax(declaringType, span, File, accessModifer,
                identifier.Span, name, selfParameter, namedParameters, @return);
        }
    }

    internal IConstructorDeclarationSyntax ParseConstructor(
        IClassDeclarationSyntax declaringType,
        ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var newKeywordSpan = Tokens.Consume<INewKeywordToken>();
        var identifier = Tokens.AcceptToken<IIdentifierToken>();
        var name = identifier is null ? null : (SimpleName)identifier.Value;
        var bodyParser = BodyParser();
        // Self parameter is expected to be after the current token which is expected to be `(`
        var expectedSelfParameterLocation = Tokens.Current.Span.AtEnd();
        var parameters = bodyParser.ParseParameters(bodyParser.ParseConstructorParameter);

        var selfParameter = parameters.OfType<IConstructorSelfParameterSyntax>().FirstOrDefault();
        var constructorParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>())
                                              .Cast<IConstructorOrInitializerParameterSyntax>()
                                        .ToFixedList();

        if (selfParameter is null)
        {
            Add(ParseError.MissingSelfParameter(File, expectedSelfParameterLocation));
            // For simplicity of downstream code, make up a fake self parameter
            var selfReferenceCapability = new CapabilitySyntax(expectedSelfParameterLocation,
                Enumerable.Empty<ICapabilityToken>(), DeclaredCapability.Mutable);
            selfParameter = new ConstructorSelfParameterSyntax(expectedSelfParameterLocation, false, selfReferenceCapability);
        }
        else if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        foreach (var extraSelfParameter in parameters.OfType<ISelfParameterSyntax>().Skip(1))
            Add(ParseError.ExtraSelfParameter(File, extraSelfParameter.Span));

        var body = bodyParser.ParseBlockBody();
        // For now, just say constructors have no annotations
        var span = TextSpan.Covering(newKeywordSpan, body.Span);
        return new ConstructorDeclarationSyntax(declaringType, span, File, accessModifer,
            TextSpan.Covering(newKeywordSpan, identifier?.Span), name, selfParameter, constructorParameters, body);
    }

    internal IInitializerDeclarationSyntax ParseInitializer(
        IStructDeclarationSyntax declaringType,
        ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var initKeywordSpan = Tokens.Consume<IInitKeywordToken>();
        var identifier = Tokens.AcceptToken<IIdentifierToken>();
        var name = identifier is null ? null : (SimpleName)identifier.Value;
        var bodyParser = BodyParser();
        // Self parameter is expected to be after the current token which is expected to be `(`
        var expectedSelfParameterLocation = Tokens.Current.Span.AtEnd();
        var parameters = bodyParser.ParseParameters(bodyParser.ParseInitializerParameter);

        var selfParameter = parameters.OfType<IInitializerSelfParameterSyntax>().FirstOrDefault();
        var constructorParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>())
                                              .Cast<IConstructorOrInitializerParameterSyntax>()
                                              .ToFixedList();

        if (selfParameter is null)
        {
            Add(ParseError.MissingSelfParameter(File, expectedSelfParameterLocation));
            // For simplicity of downstream code, make up a fake self parameter
            var selfReferenceCapability = new CapabilitySyntax(expectedSelfParameterLocation,
                Enumerable.Empty<ICapabilityToken>(), DeclaredCapability.Mutable);
            selfParameter = new InitializerSelfParameterSyntax(expectedSelfParameterLocation, false, selfReferenceCapability);
        }
        else if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        foreach (var extraSelfParameter in parameters.OfType<ISelfParameterSyntax>().Skip(1))
            Add(ParseError.ExtraSelfParameter(File, extraSelfParameter.Span));

        var body = bodyParser.ParseBlockBody();
        // For now, just say constructors have no annotations
        var span = TextSpan.Covering(initKeywordSpan, body.Span);
        return new InitializerDeclarationSyntax(declaringType, span, File, accessModifer,
            TextSpan.Covering(initKeywordSpan, identifier?.Span), name, selfParameter, constructorParameters, body);
    }

    internal IStructMemberDeclarationSyntax ParseStructMemberFunction(
        IStructDeclarationSyntax declaringType,
        ModifierParser modifiers)
    {
        var accessModifer = modifiers.ParseAccessModifier();
        modifiers.ParseEndOfModifiers();
        var fn = Tokens.Consume<IFunctionKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        SimpleName name = identifier.Value;
        var bodyParser = BodyParser();
        var parameters = bodyParser.ParseParameters(bodyParser.ParseMethodParameter);
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
                body = bodyParser.ParseBody();
                span = TextSpan.Covering(fn, body.Span);
            }
            else
            {
                body = new BlockBodySyntax(Tokens.Current.Span.AtStart(), FixedList.Empty<IBodyStatementSyntax>());
                var semicolon = bodyParser.Tokens.Expect<ISemicolonToken>();
                span = TextSpan.Covering(fn, semicolon);
                Add(ParseError.AssociatedFunctionMissingBody(File, span, name));
            }

            return new AssociatedFunctionDeclarationSyntax(declaringType, span, File, accessModifer, identifier.Span, name, namedParameters, @return, body);
        }

        if (parameters[0] is not ISelfParameterSyntax)
            Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

        foreach (var extraSelfParameter in parameters.OfType<ISelfParameterSyntax>().Skip(1))
            Add(ParseError.ExtraSelfParameter(File, extraSelfParameter.Span));

        // It is a method that may or may not have a body
        if (Tokens.Current is IOpenBraceToken or IRightDoubleArrowToken)
        {
            body = bodyParser.ParseBody();
            span = TextSpan.Covering(fn, body.Span);
        }
        else
        {
            body = new BlockBodySyntax(Tokens.Current.Span.AtStart(), FixedList.Empty<IBodyStatementSyntax>());
            var semicolon = bodyParser.Tokens.Expect<ISemicolonToken>();
            span = TextSpan.Covering(fn, semicolon);
            Add(ParseError.StructMethodMissingBody(File, span, name));
        }

        return new ConcreteMethodDeclarationSyntax(declaringType, span, File, accessModifer,
            identifier.Span, name, selfParameter, namedParameters, @return, body);
    }
    #endregion
}
