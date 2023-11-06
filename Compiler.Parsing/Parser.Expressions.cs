using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using UnaryOperator = Azoth.Tools.Bootstrap.Compiler.Core.Operators.UnaryOperator;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    public IExpressionSyntax? AcceptExpression()
    {
        try
        {
            return Tokens.Current switch
            {
                ICloseParenToken
                    or ICloseBraceToken
                    or ISemicolonToken
                    or ICommaToken
                    or IRightArrowToken => null,
                _ => ParseExpression()
            };
        }
        catch (ParseFailedException)
        {
            return null;
        }
    }

    public IExpressionSyntax ParseExpression() => ParseExpression(OperatorPrecedence.Min);

    /// <summary>
    /// For expressions, we switch to a precedence climbing parser.
    /// </summary>
    public IExpressionSyntax ParseExpression(OperatorPrecedence minPrecedence)
    {
        var expression = ParseAtom();

        for (; ; )
        {
            IBinaryOperatorToken? @operator = null;
            OperatorPrecedence? precedence = null;
            var leftAssociative = true;
            switch (Tokens.Current)
            {
                case IEqualsToken _:
                case IPlusEqualsToken _:
                case IMinusEqualsToken _:
                case IAsteriskEqualsToken _:
                case ISlashEqualsToken _:
                    if (minPrecedence <= OperatorPrecedence.Assignment)
                    {
                        var assignmentOperator = BuildAssignmentOperator(Tokens.RequiredToken<IAssignmentToken>());
                        var rightOperand = ParseExpression();
                        if (expression is IAssignableExpressionSyntax assignableExpression)
                            expression = new AssignmentExpressionSyntax(assignableExpression, assignmentOperator, rightOperand);
                        else
                            // Can't assign expression, so it is just the right hand side of the assignment
                            Add(ParseError.CantAssignIntoExpression(File, expression.Span));
                        continue;
                    }
                    break;
                case IQuestionQuestionToken _:
                    if (minPrecedence <= OperatorPrecedence.Coalesce)
                    {
                        precedence = OperatorPrecedence.Coalesce;
                        @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                    }
                    break;
                case IOrKeywordToken _:
                    if (minPrecedence <= OperatorPrecedence.LogicalOr)
                    {
                        precedence = OperatorPrecedence.LogicalOr;
                        @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                    }
                    break;
                case IAndKeywordToken _:
                    if (minPrecedence <= OperatorPrecedence.LogicalAnd)
                    {
                        precedence = OperatorPrecedence.LogicalAnd;
                        @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                    }
                    break;
                case IEqualsEqualsToken _:
                case INotEqualToken _:
                    if (minPrecedence <= OperatorPrecedence.Equality)
                    {
                        precedence = OperatorPrecedence.Equality;
                        @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                    }
                    break;
                case ILessThanToken _:
                case ILessThanOrEqualToken _:
                case IGreaterThanToken _:
                case IGreaterThanOrEqualToken _:
                case ILessThanColonToken _: // Subtype operator
                    if (minPrecedence <= OperatorPrecedence.Relational)
                    {
                        precedence = OperatorPrecedence.Relational;
                        @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                    }
                    break;
                case IDotDotToken _:
                case ILessThanDotDotToken _:
                case IDotDotLessThanToken _:
                case ILessThanDotDotLessThanToken _:
                    if (minPrecedence <= OperatorPrecedence.Range)
                    {
                        precedence = OperatorPrecedence.Range;
                        @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                    }
                    break;
                case IAsKeywordToken _:
                case IAsExclamationKeywordToken _:
                case IAsQuestionKeywordToken _:
                    if (minPrecedence <= OperatorPrecedence.Conversion)
                    {
                        var conversionOperator = BuildConversionOperator(Tokens.RequiredToken<IConversionOperatorToken>());
                        var typeSyntax = ParseType();
                        expression = new ConversionExpressionSyntax(expression, conversionOperator, typeSyntax);
                        continue;
                    }
                    break;
                case IIsKeywordToken _:
                    if (minPrecedence <= OperatorPrecedence.Relational)
                    {
                        _ = Tokens.Required<IIsKeywordToken>();
                        var patternSyntax = ParsePattern();
                        expression = new PatternMatchExpressionSyntax(expression, patternSyntax);
                        continue;
                    }
                    break;
                case IPlusToken _:
                case IMinusToken _:
                    if (minPrecedence <= OperatorPrecedence.Additive)
                    {
                        precedence = OperatorPrecedence.Additive;
                        @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                    }
                    break;
                case IAsteriskToken _:
                case ISlashToken _:
                    if (minPrecedence <= OperatorPrecedence.Multiplicative)
                    {
                        precedence = OperatorPrecedence.Multiplicative;
                        @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                    }
                    break;
                case IDotToken _:
                case IQuestionDotToken _:
                    if (minPrecedence <= OperatorPrecedence.Primary)
                    {
                        // Member Access
                        var accessOperator = BuildAccessOperator(Tokens.RequiredToken<IAccessOperatorToken>());
                        var nameSyntax = ParseName();
                        var memberAccessSpan = TextSpan.Covering(expression.Span, nameSyntax.Span);
                        expression = new QualifiedNameExpressionSyntax(memberAccessSpan, expression, accessOperator, nameSyntax);
                        if (Tokens.Current is IOpenParenToken)
                        {
                            Tokens.RequiredToken<IOpenParenToken>();
                            var arguments = ParseArguments();
                            var closeParenSpan = Tokens.Expect<ICloseParenToken>();
                            var invocationSpan = TextSpan.Covering(expression.Span, closeParenSpan);
                            expression = new InvocationExpressionSyntax(invocationSpan, expression, arguments);
                        }
                        continue;
                    }
                    break;
                case ISemicolonToken _:
                case ICloseBraceToken _:
                    // These terminating tokens should be safe to return the atom even if it is a missing identifier
                    return expression;
                default:
                    if (expression is ISimpleNameExpressionSyntax { Name: null })
                    {
                        // Weren't able to parse an atom nor find an operator. There is a risk of
                        // not making progress. Mark the next token as unexpected.
                        Tokens.UnexpectedToken();
                    }
                    return expression;
            }

            if (@operator is not null &&
                precedence is OperatorPrecedence operatorPrecedence)
            {
                if (leftAssociative)
                    operatorPrecedence += 1;

                var rightOperand = ParseExpression(operatorPrecedence);
                expression = BuildBinaryOperatorExpression(expression, @operator, rightOperand);
            }
            else
            {
                // if we didn't match any operator
                return expression;
            }
        }
    }

    private static AssignmentOperator BuildAssignmentOperator(IAssignmentToken assignmentToken)
    {
        return assignmentToken switch
        {
            IEqualsToken _ => AssignmentOperator.Simple,
            IPlusEqualsToken _ => AssignmentOperator.Plus,
            IMinusEqualsToken _ => AssignmentOperator.Minus,
            IAsteriskEqualsToken _ => AssignmentOperator.Asterisk,
            ISlashEqualsToken _ => AssignmentOperator.Slash,
            _ => throw ExhaustiveMatch.Failed(assignmentToken)
        };
    }

    private static AccessOperator BuildAccessOperator(IAccessOperatorToken accessOperatorToken)
    {
        return accessOperatorToken switch
        {
            IDotToken _ => AccessOperator.Standard,
            IQuestionDotToken _ => AccessOperator.Conditional,
            _ => throw ExhaustiveMatch.Failed(accessOperatorToken)
        };
    }

    private static ConversionOperator BuildConversionOperator(IConversionOperatorToken conversionOperatorToken)
    {
        return conversionOperatorToken switch
        {
            IAsKeywordToken _ => ConversionOperator.Safe,
            IAsExclamationKeywordToken _ => ConversionOperator.Aborting,
            IAsQuestionKeywordToken _ => ConversionOperator.Optional,
            _ => throw ExhaustiveMatch.Failed(conversionOperatorToken)
        };
    }

    private static IExpressionSyntax BuildBinaryOperatorExpression(
        IExpressionSyntax left,
        IBinaryOperatorToken operatorToken,
        IExpressionSyntax right)
    {
        BinaryOperator binaryOperator = operatorToken switch
        {
            IPlusToken _ => BinaryOperator.Plus,
            IMinusToken _ => BinaryOperator.Minus,
            IAsteriskToken _ => BinaryOperator.Asterisk,
            ISlashToken _ => BinaryOperator.Slash,
            IEqualsEqualsToken _ => BinaryOperator.EqualsEquals,
            INotEqualToken _ => BinaryOperator.NotEqual,
            ILessThanToken _ => BinaryOperator.LessThan,
            ILessThanOrEqualToken _ => BinaryOperator.LessThanOrEqual,
            IGreaterThanToken _ => BinaryOperator.GreaterThan,
            IGreaterThanOrEqualToken _ => BinaryOperator.GreaterThanOrEqual,
            IAndKeywordToken _ => BinaryOperator.And,
            IOrKeywordToken _ => BinaryOperator.Or,
            IDotDotToken _ => BinaryOperator.DotDot,
            ILessThanDotDotToken _ => BinaryOperator.LessThanDotDot,
            IDotDotLessThanToken _ => BinaryOperator.DotDotLessThan,
            ILessThanDotDotLessThanToken _ => BinaryOperator.LessThanDotDotLessThan,
            IQuestionQuestionToken _ => BinaryOperator.QuestionQuestion,
            _ => throw ExhaustiveMatch.Failed(operatorToken)
        };
        return new BinaryOperatorExpressionSyntax(left, binaryOperator, right);
    }

    // An atom is the unit of an expression that occurs between infix operators, i.e. an identifier, literal, group, or new
    private IExpressionSyntax ParseAtom()
    {
        switch (Tokens.Current)
        {
            default:
                throw ExhaustiveMatch.Failed(Tokens.Current);
            case ISelfKeywordToken _:
                return ParseSelfExpression();
            case INewKeywordToken _:
            {
                var newKeyword = Tokens.Expect<INewKeywordToken>();
                var type = ParseTypeName();
                Tokens.Expect<IOpenParenToken>();
                var arguments = ParseArguments();
                var closeParen = Tokens.Expect<ICloseParenToken>();
                var span = TextSpan.Covering(newKeyword, closeParen);
                return new NewObjectExpressionSyntax(span, type, null, null, arguments);
            }
            case IReturnKeywordToken _:
            {
                var returnKeyword = Tokens.Expect<IReturnKeywordToken>();
                var expression = Tokens.AtEnd<ISemicolonToken>() ? null : ParseExpression();
                var span = TextSpan.Covering(returnKeyword, expression?.Span);
                return new ReturnExpressionSyntax(span, expression);
            }
            case IOpenParenToken _:
                return ParseParenthesizedExpression();
            case IPlusToken _:
                return ParsePrefixUnaryOperator(UnaryOperator.Plus);
            case IMinusToken _:
                return ParsePrefixUnaryOperator(UnaryOperator.Minus);
            case INotKeywordToken _:
                return ParsePrefixUnaryOperator(UnaryOperator.Not);
            case IBooleanLiteralToken _:
            {
                var literal = Tokens.RequiredToken<IBooleanLiteralToken>();
                return new BoolLiteralExpressionSyntax(literal.Span, literal.Value);
            }
            case IIntegerLiteralToken _:
            {
                var literal = Tokens.RequiredToken<IIntegerLiteralToken>();
                return new IntegerLiteralExpressionSyntax(literal.Span, literal.Value);
            }
            case IStringLiteralToken _:
            {
                var literal = Tokens.RequiredToken<IStringLiteralToken>();
                return new StringLiteralExpressionSyntax(literal.Span, literal.Value);
            }
            case INoneKeywordToken _:
            {
                var literal = Tokens.Required<INoneKeywordToken>();
                return new NoneLiteralExpressionSyntax(literal);
            }
            case IIdentifierToken _:
            {
                var nameSyntax = ParseName();
                if (Tokens.Current is not IOpenParenToken)
                    return nameSyntax;
                Tokens.RequiredToken<IOpenParenToken>();
                var arguments = ParseArguments();
                var closeParenSpan = Tokens.Expect<ICloseParenToken>();
                var span = TextSpan.Covering(nameSyntax.Span, closeParenSpan);
                return new InvocationExpressionSyntax(span, nameSyntax, arguments);
            }
            case IForeachKeywordToken _:
                return ParseForeach();
            case IWhileKeywordToken _:
                return ParseWhile();
            case ILoopKeywordToken _:
                return ParseLoop();
            case IBreakKeywordToken _:
            {
                var breakKeyword = Tokens.Expect<IBreakKeywordToken>();
                // TODO parse label
                var expression = AcceptExpression();
                var span = TextSpan.Covering(breakKeyword, expression?.Span);
                return new BreakExpressionSyntax(span, expression);
            }
            case INextKeywordToken _:
            {
                var span = Tokens.Required<INextKeywordToken>();
                return new NextExpressionSyntax(span);
            }
            case IUnsafeKeywordToken _:
                return ParseUnsafeExpression();
            case IIfKeywordToken _:
                return ParseIf();
            case IDotToken dot:
            {
                // implicit self, don't consume the '.' it will be parsed next
                return new SelfExpressionSyntax(dot.Span.AtStart(), true);
            }
            case IIdKeywordToken _:
            {
                var id = Tokens.Required<IIdKeywordToken>();
                // `id` is like a unary operator
                var expression = ParseExpression(OperatorPrecedence.Unary);
                var span = TextSpan.Covering(id, expression.Span);
                return new IdExpressionSyntax(span, expression);
            }
            case IMoveKeywordToken _:
            {
                var move = Tokens.Required<IMoveKeywordToken>();
                // `move` is like a unary operator
                var expression = ParseExpression(OperatorPrecedence.Unary);
                var span = TextSpan.Covering(move, expression.Span);
                if (expression is ISimpleNameExpressionSyntax name)
                    return new MoveExpressionSyntax(span, name);
                if(expression is ISelfExpressionSyntax self)
                    return new MoveExpressionSyntax(span, self);
                Add(ParseError.CantMoveOutOfExpression(File, span));
                return expression;
            }
            case IFreezeKeywordToken _:
            {
                var freeze = Tokens.Required<IFreezeKeywordToken>();
                // `freeze` is like a unary operator
                var expression = ParseExpression(OperatorPrecedence.Unary);
                var span = TextSpan.Covering(freeze, expression.Span);
                if (expression is ISimpleNameExpressionSyntax name)
                    return new FreezeExpressionSyntax(span, name);
                Add(ParseError.CantFreezeExpression(File, span));
                return expression;
            }
            case IOpenBraceToken _:
                return ParseBlock();
            case IBinaryOperatorToken _:
            case IAssignmentToken _:
            case IQuestionDotToken _:
            case ISemicolonToken _:
            case ICloseParenToken _:
                // If it is one of these, we assume there is a missing identifier
                return ParseMissingIdentifier();
            case ICloseBraceToken _:
            case IColonToken _:
            case IColonColonDotToken _:
            case ILessThanColonToken _:
            case ICommaToken _:
            case IRightArrowToken _:
            case IQuestionToken _:
            case IKeywordToken _:
            case IEndOfFileToken _:
            case IOpenBracketToken _:
            case ICloseBracketToken _:
            case IHashToken _:
                Add(ParseError.UnexpectedEndOfExpression(File, Tokens.Current.Span.AtStart()));
                throw new ParseFailedException("Unexpected end of expression");
            case IRightDoubleArrowToken _:
                throw new NotImplementedException($"`{Tokens.Current.Text(File.Code)}` in expression position");
        }
    }

    private ISelfExpressionSyntax ParseSelfExpression()
    {
        var selfKeyword = Tokens.Expect<ISelfKeywordToken>();
        return new SelfExpressionSyntax(selfKeyword, false);
    }

    private ISimpleNameExpressionSyntax ParseMissingIdentifier()
    {
        var identifierSpan = Tokens.Expect<IIdentifierToken>();
        return new SimpleNameExpressionSyntax(identifierSpan, null);
    }

    private IUnsafeExpressionSyntax ParseUnsafeExpression()
    {
        var unsafeKeyword = Tokens.Expect<IUnsafeKeywordToken>();
        var isBlock = Tokens.Current is IOpenBraceToken;
        var expression = isBlock
            ? ParseBlock()
            : ParseParenthesizedExpression();
        var span = TextSpan.Covering(unsafeKeyword, expression.Span);
        return new UnsafeExpressionSyntax(span, expression);
    }

    private IUnaryOperatorExpressionSyntax ParsePrefixUnaryOperator(UnaryOperator @operator)
    {
        var operatorSpan = Tokens.Required<IOperatorToken>();
        var operand = ParseExpression(OperatorPrecedence.Unary);
        var span = TextSpan.Covering(operatorSpan, operand.Span);
        return new UnaryOperatorExpressionSyntax(span, UnaryOperatorFixity.Prefix, @operator, operand);
    }

    private IForeachExpressionSyntax ParseForeach()
    {
        var foreachKeyword = Tokens.Expect<IForeachKeywordToken>();
        var mutableBinding = Tokens.Accept<IVarKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var variableName = identifier.Value;
        ITypeSyntax? type = null;
        if (Tokens.Accept<IColonToken>())
            type = ParseType();
        Tokens.Expect<IInKeywordToken>();
        var expression = ParseExpression();
        var block = ParseBlock();
        var span = TextSpan.Covering(foreachKeyword, block.Span);
        return new ForeachExpressionSyntax(span, mutableBinding, variableName, type, expression, block);
    }

    private IWhileExpressionSyntax ParseWhile()
    {
        var whileKeyword = Tokens.Expect<IWhileKeywordToken>();
        var condition = ParseExpression();
        var block = ParseBlock();
        var span = TextSpan.Covering(whileKeyword, block.Span);
        return new WhileExpressionSyntax(span, condition, block);
    }

    private ILoopExpressionSyntax ParseLoop()
    {
        var loopKeyword = Tokens.Expect<ILoopKeywordToken>();
        var block = ParseBlock();
        var span = TextSpan.Covering(loopKeyword, block.Span);
        return new LoopExpressionSyntax(span, block);
    }

    private IIfExpressionSyntax ParseIf(ParseAs parseAs = ParseAs.Expression)
    {
        var @if = Tokens.Expect<IIfKeywordToken>();
        var condition = ParseExpression();
        var thenBlock = ParseBlockOrResultExpression();
        var elseClause = AcceptElse(parseAs);
        var span = TextSpan.Covering(@if, thenBlock.Span, elseClause?.Span);
        if (parseAs == ParseAs.Statement
            && elseClause is null
            && thenBlock is IResultStatementSyntax)
        {
            var semicolon = Tokens.Expect<ISemicolonToken>();
            span = TextSpan.Covering(span, semicolon);
        }
        return new IfExpressionSyntax(span, condition, thenBlock, elseClause);
    }

    private IElseClauseSyntax? AcceptElse(ParseAs parseAs)
    {
        if (!Tokens.Accept<IElseKeywordToken>())
            return null;
        var expression = Tokens.Current is IIfKeywordToken
            ? (IElseClauseSyntax)ParseIf(parseAs)
            : ParseBlockOrResultExpression();
        if (parseAs == ParseAs.Statement
            && expression is IResultStatementSyntax)
            Tokens.Expect<ISemicolonToken>();
        return expression;
    }

    /// <summary>
    /// Parse an expression that is required to have parenthesis around it.
    /// for example `unsafe(x);`.
    /// </summary>
    private IExpressionSyntax ParseParenthesizedExpression()
    {
        Tokens.Expect<IOpenParenToken>();
        var expression = ParseExpression();
        Tokens.Expect<ICloseParenToken>();
        return expression;
    }

    public FixedList<IExpressionSyntax> ParseArguments()
        => AcceptManySeparated<IExpressionSyntax, ICommaToken>(AcceptExpression);

    public IBlockOrResultSyntax ParseBlockOrResultExpression()
    {
        if (Tokens.Current is IOpenBraceToken)
            return ParseBlock();

        return ParseResultExpression();
    }

    private IResultStatementSyntax ParseResultExpression()
    {
        var rightDoubleArrow = Tokens.Expect<IRightDoubleArrowToken>();
        var expression = ParseExpression();
        var span = TextSpan.Covering(rightDoubleArrow, expression.Span);
        return new ResultStatementSyntax(span, expression);
    }
}
