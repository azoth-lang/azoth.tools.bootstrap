using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    public IStatementSyntax ParseStatement()
    {
        switch (Tokens.Current)
        {
            case IOpenBraceToken _:
                var block = ParseBlock();
                return IExpressionStatementSyntax.Create(block.Span, block);
            case ILetKeywordToken _:
                var let = Tokens.Consume<IBindingToken>();
                return ParseRestOfVariableDeclaration(let, false);
            case IVarKeywordToken _:
                var var = Tokens.Consume<IBindingToken>();
                return ParseRestOfVariableDeclaration(var, true);
            case IForeachKeywordToken _:
                var @foreach = ParseForeach();
                return IExpressionStatementSyntax.Create(@foreach.Span, @foreach);
            case IWhileKeywordToken _:
                var @while = ParseWhile();
                return IExpressionStatementSyntax.Create(@while.Span, @while);
            case ILoopKeywordToken _:
                var loop = ParseLoop();
                return IExpressionStatementSyntax.Create(loop.Span, loop);
            case IIfKeywordToken _:
                var @if = ParseIf(ParseAs.Statement);
                return IExpressionStatementSyntax.Create(@if.Span, @if);
            case IAsyncKeywordToken _:
                var async = ParseAsyncBlock();
                return IExpressionStatementSyntax.Create(async.Span, async);
            case IUnsafeKeywordToken _:
                return ParseUnsafeStatement();
            case IRightDoubleArrowToken _:
                return ParseResultStatement();
            default:
                try
                {
                    var expression = ParseExpression();
                    var semicolon = Tokens.Expect<ISemicolonToken>();
                    return IExpressionStatementSyntax.Create(
                        TextSpan.Covering(expression.Span, semicolon), expression);
                }
                catch (ParseFailedException)
                {
                    SkipToEndOfStatement();
                    throw;
                }
        }
    }

    // Requires the binding has already been consumed
    private IStatementSyntax ParseRestOfVariableDeclaration(
        TextSpan binding,
        bool isMutableBinding)
    {
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var name = identifier.Value;
        ITypeSyntax? type = null;
        ICapabilitySyntax? capability = null;
        if (Tokens.Accept<IColonToken>())
            (type, capability) = ParseVariableDeclarationType();

        IExpressionSyntax? initializer = null;
        if (Tokens.Accept<IEqualsToken>())
            initializer = ParseExpression();

        var semicolon = Tokens.Expect<ISemicolonToken>();
        var span = TextSpan.Covering(binding, semicolon);
        return IVariableDeclarationStatementSyntax.Create(span,
            isMutableBinding, identifier.Span, name, capability, type, initializer);
    }

    // TODO return is really an either type
    private (ITypeSyntax? Type, ICapabilitySyntax? Capability) ParseVariableDeclarationType()
    {
        var capability = AcceptStandardCapability();

        switch (Tokens.Current)
        {
            case IEqualsToken _:
            case ISemicolonToken _:
                if (capability is null)
                    // Error on `let x := ...` or `let x :;`
                    Add(ParseError.MissingType(File, Tokens.Current.Span.AtStart()));
                return (null, capability);
            default:
                return (ParseTypeStartingWith(capability), null);
        }
    }

    private IExpressionStatementSyntax ParseUnsafeStatement()
    {
        var unsafeKeyword = Tokens.Consume<IUnsafeKeywordToken>();
        var isBlock = Tokens.Current is IOpenBraceToken;
        var expression = isBlock ? ParseBlock() : ParseParenthesizedExpression();
        var span = TextSpan.Covering(unsafeKeyword, expression.Span);
        var unsafeExpression = IUnsafeExpressionSyntax.Create(span, expression);
        if (!isBlock)
        {
            var semicolon = Tokens.Expect<ISemicolonToken>();
            span = TextSpan.Covering(span, semicolon);
        }
        return IExpressionStatementSyntax.Create(span, unsafeExpression);
    }

    public IBlockExpressionSyntax ParseBlock()
    {
        var openBrace = Tokens.Expect<IOpenBraceToken>();
        var statements = ParseMany<IStatementSyntax, ICloseBraceToken>(ParseStatement);
        var closeBrace = Tokens.Expect<ICloseBraceToken>();
        var span = TextSpan.Covering(openBrace, closeBrace);
        return IBlockExpressionSyntax.Create(span, statements);
    }

    private IResultStatementSyntax ParseResultStatement()
    {
        try
        {
            var rightDoubleArrow = Tokens.Expect<IRightDoubleArrowToken>();
            var expression = ParseExpression();
            var semicolon = Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(rightDoubleArrow, expression.Span, semicolon);
            return IResultStatementSyntax.Create(span, expression);
        }
        catch (ParseFailedException)
        {
            SkipToEndOfStatement();
            throw;
        }
    }

    /// <summary>
    /// Skip tokens until we reach what we assume to be the end of a statement
    /// </summary>
    private void SkipToEndOfStatement()
    {
        while (!Tokens.AtEnd<ISemicolonToken>())
            Tokens.Next();

        // Consume the semicolon if we aren't at the end of the file.
        _ = Tokens.Accept<ISemicolonToken>();
    }
}
