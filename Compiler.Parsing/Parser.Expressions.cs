using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
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
                        var assignmentOperator = BuildAssignmentOperator(Tokens.ConsumeToken<IAssignmentOperatorToken>());
                        var rightOperand = ParseExpression();
                        var span = TextSpan.Covering(expression.Span, rightOperand.Span);
                        expression = IAssignmentExpressionSyntax.Create(span, expression,
                            assignmentOperator, rightOperand);
                        continue;
                    }
                    break;
                case IQuestionQuestionToken _:
                    if (minPrecedence <= OperatorPrecedence.Coalesce)
                    {
                        precedence = OperatorPrecedence.Coalesce;
                        @operator = Tokens.ConsumeToken<IBinaryOperatorToken>();
                    }
                    break;
                case IOrKeywordToken _:
                    if (minPrecedence <= OperatorPrecedence.LogicalOr)
                    {
                        precedence = OperatorPrecedence.LogicalOr;
                        @operator = Tokens.ConsumeToken<IBinaryOperatorToken>();
                    }
                    break;
                case IAndKeywordToken _:
                    if (minPrecedence <= OperatorPrecedence.LogicalAnd)
                    {
                        precedence = OperatorPrecedence.LogicalAnd;
                        @operator = Tokens.ConsumeToken<IBinaryOperatorToken>();
                    }
                    break;
                case IEqualsEqualsToken _:
                case INotEqualToken _:
                    if (minPrecedence <= OperatorPrecedence.Equality)
                    {
                        precedence = OperatorPrecedence.Equality;
                        @operator = Tokens.ConsumeToken<IBinaryOperatorToken>();
                    }
                    break;
                case ILessThanToken _:
                case ILessThanOrEqualToken _:
                case IGreaterThanToken _:
                case IGreaterThanOrEqualToken _:
                case IReferenceEqualsToken _:
                case INotReferenceEqualsToken _:
                case ILessThanColonToken _: // Subtype operator
                    if (minPrecedence <= OperatorPrecedence.Relational)
                    {
                        precedence = OperatorPrecedence.Relational;
                        @operator = Tokens.ConsumeToken<IBinaryOperatorToken>();
                    }
                    break;
                case IDotDotToken _:
                case ILessThanDotDotToken _:
                case IDotDotLessThanToken _:
                case ILessThanDotDotLessThanToken _:
                    if (minPrecedence <= OperatorPrecedence.Range)
                    {
                        precedence = OperatorPrecedence.Range;
                        @operator = Tokens.ConsumeToken<IBinaryOperatorToken>();
                    }
                    break;
                case IAsKeywordToken _:
                case IAsExclamationKeywordToken _:
                case IAsQuestionKeywordToken _:
                    if (minPrecedence <= OperatorPrecedence.Conversion)
                    {
                        var conversionOperator = BuildConversionOperator(Tokens.ConsumeToken<IConversionOperatorToken>());
                        var type = ParseType();
                        var span = TextSpan.Covering(expression.Span, type.Span);
                        expression = IConversionExpressionSyntax.Create(span, expression, conversionOperator, type);
                        continue;
                    }
                    break;
                case IIsKeywordToken _:
                    if (minPrecedence <= OperatorPrecedence.Relational)
                    {
                        _ = Tokens.Consume<IIsKeywordToken>();
                        var pattern = ParsePattern();
                        var span = TextSpan.Covering(expression.Span, pattern.Span);
                        expression = IPatternMatchExpressionSyntax.Create(span, expression, pattern);
                        continue;
                    }
                    break;
                case IPlusToken _:
                case IMinusToken _:
                    if (minPrecedence <= OperatorPrecedence.Additive)
                    {
                        precedence = OperatorPrecedence.Additive;
                        @operator = Tokens.ConsumeToken<IBinaryOperatorToken>();
                    }
                    break;
                case IAsteriskToken _:
                case ISlashToken _:
                    if (minPrecedence <= OperatorPrecedence.Multiplicative)
                    {
                        precedence = OperatorPrecedence.Multiplicative;
                        @operator = Tokens.ConsumeToken<IBinaryOperatorToken>();
                    }
                    break;
                case IDotToken _:
                    if (minPrecedence <= OperatorPrecedence.Primary)
                    {
                        // Member Access
                        Tokens.ConsumeToken<IAccessOperatorToken>();
                        var memberName = ParseOrdinaryName();
                        var memberAccessSpan = TextSpan.Covering(expression.Span, memberName.Span);
                        expression = expression is INameSyntax nameExpression
                            ? IQualifiedNameSyntax.Create(memberAccessSpan, nameExpression,
                                memberName.Span, memberName.Name, memberName.GenericArguments)
                            : IMemberAccessExpressionSyntax.Create(memberAccessSpan, expression,
                                memberName.Span, memberName.Name, memberName.GenericArguments);
                        continue;
                    }
                    break;
                case IOpenParenToken _:
                    if (minPrecedence <= OperatorPrecedence.Primary)
                    {
                        Tokens.Consume<IOpenParenToken>();
                        var arguments = ParseArguments();
                        var closeParenSpan = Tokens.Expect<ICloseParenToken>();
                        var invocationSpan = TextSpan.Covering(expression.Span, closeParenSpan);
                        expression = IInvocationExpressionSyntax.Create(invocationSpan, expression, arguments);
                        continue;
                    }
                    break;
                case ISemicolonToken _:
                case ICloseBraceToken _:
                    // These terminating tokens should be safe to return the atom even if it is a missing identifier
                    return expression;
                default:
                    if (expression is IMissingNameExpressionSyntax)
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

    private static AssignmentOperator BuildAssignmentOperator(IAssignmentOperatorToken assignmentToken)
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
            IReferenceEqualsToken _ => BinaryOperator.ReferenceEquals,
            INotReferenceEqualsToken _ => BinaryOperator.NotReferenceEqual,
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
        var span = TextSpan.Covering(left.Span, right.Span);
        return IBinaryOperatorExpressionSyntax.Create(span, left, binaryOperator, right);
    }

    // An atom is the unit of an expression that occurs between infix operators, i.e. an identifier, literal, group, or new
    private IExpressionSyntax ParseAtom()
    {
        switch (Tokens.Current)
        {
            default:
                throw ExhaustiveMatch.Failed(Tokens.Current);
            case ICloseBraceToken _:
            case IColonToken _:
            case IColonColonToken _:
            case IColonColonDotToken _:
            case ILessThanColonToken _:
            case ICommaToken _:
            case IRightArrowToken _:
            case IQuestionToken _:
            case IEndOfFileToken _:
            case IOpenBracketToken _:
            case ICloseBracketToken _:
            case IHashToken _:
            case IRightTriangleToken _:
            // Various keyword tokens are listed explicitly here rather than using IKeywordToken so
            // that exhaustive matching will properly report when an added keyword ought to have
            // expression parsing.
            case IBindingToken _:
            case IAccessModifierToken _:
            case ITypeKindKeywordToken _:
            case ICapabilityToken _:
            case ICapabilitySetToken _:
            case IConversionOperatorToken _:
            case IAbstractKeywordToken _:
            case ICopyKeywordToken _:
            case IElseKeywordToken _:
            case IInitKeywordToken _:
            case ILentKeywordToken _:
            case IInKeywordToken _:
            case IOutKeywordToken _:
            case INonwritableKeywordToken _:
            case IFunctionKeywordToken _:
            case IImportKeywordToken _:
            case IGetKeywordToken _:
            case ISetKeywordToken _:
            case ISafeKeywordToken _:
            case INamespaceKeywordToken _:
            case IIndependentKeywordToken _:
            case IIsKeywordToken _:
                Add(ParseError.UnexpectedEndOfExpression(File, Tokens.Current.Span.AtStart()));
                throw new ParseFailedException("Unexpected end of expression");
            case ISelfKeywordToken _:
                return ParseSelfExpression();
            case IReturnKeywordToken _:
            {
                var returnKeyword = Tokens.Consume<IReturnKeywordToken>();
                var expression = Tokens.AtEnd<ISemicolonToken>() ? null : ParseExpression();
                var span = TextSpan.Covering(returnKeyword, expression?.Span);
                return IReturnExpressionSyntax.Create(span, expression);
            }
            case IOpenParenToken _:
                return ParseParenthesizedExpression();
            case IPlusToken _:
                return ParsePrefixUnaryOperator(UnaryOperator.Plus);
            case IMinusToken _:
                return ParsePrefixUnaryOperator(UnaryOperator.Minus);
            case INotKeywordToken _:
                // TODO fix precedence to be LogicalNot
                return ParsePrefixUnaryOperator(UnaryOperator.Not);
            case IBooleanLiteralToken _:
            {
                var literal = Tokens.ConsumeToken<IBooleanLiteralToken>();
                return IBoolLiteralExpressionSyntax.Create(literal.Span, literal.Value);
            }
            case IIntegerLiteralToken _:
            {
                var literal = Tokens.ConsumeToken<IIntegerLiteralToken>();
                return IIntegerLiteralExpressionSyntax.Create(literal.Span, literal.Value);
            }
            case IStringLiteralToken _:
            {
                var literal = Tokens.ConsumeToken<IStringLiteralToken>();
                return IStringLiteralExpressionSyntax.Create(literal.Span, literal.Value);
            }
            case INoneKeywordToken _:
            {
                var literal = Tokens.Consume<INoneKeywordToken>();
                return INoneLiteralExpressionSyntax.Create(literal);
            }
            case IIdentifierToken _:
                return ParseOrdinaryName();
            case IBuiltInTypeToken _:
                return ParseBuiltInType();
            case ISelfTypeKeywordToken _:
                return ParseSelfType();
            case IForeachKeywordToken _:
                return ParseForeach();
            case IWhileKeywordToken _:
                return ParseWhile();
            case ILoopKeywordToken _:
                return ParseLoop();
            case IBreakKeywordToken _:
            {
                var breakKeyword = Tokens.Consume<IBreakKeywordToken>();
                // TODO parse label
                var expression = AcceptExpression();
                var span = TextSpan.Covering(breakKeyword, expression?.Span);
                return IBreakExpressionSyntax.Create(span, expression);
            }
            case INextKeywordToken _:
            {
                var span = Tokens.Consume<INextKeywordToken>();
                return INextExpressionSyntax.Create(span);
            }
            case IUnsafeKeywordToken _:
                return ParseUnsafeExpression();
            case IIfKeywordToken _:
                return ParseIf();
            case IDotToken dot:
            {
                // implicit self, don't consume the '.' it will be parsed next
                return ISelfExpressionSyntax.Create(dot.Span.AtStart(), true);
            }
            case IMoveKeywordToken _:
            {
                var move = Tokens.Consume<IMoveKeywordToken>();
                // `move` is like a unary operator
                var expression = ParseExpression(OperatorPrecedence.Unary);
                var span = TextSpan.Covering(move, expression.Span);
                return IMoveExpressionSyntax.Create(span, expression);
            }
            case IFreezeKeywordToken _:
            {
                var freeze = Tokens.Consume<IFreezeKeywordToken>();
                // `freeze` is like a unary operator
                var expression = ParseExpression(OperatorPrecedence.Unary);
                var span = TextSpan.Covering(freeze, expression.Span);
                return IFreezeExpressionSyntax.Create(span, expression);
            }
            case IRefKeywordToken _:
            {
                var refToken = Tokens.ConsumeToken<IRefKeywordToken>();
                var isInternal = refToken is IInternalRefKeywordToken;
                var isVarBinding = Tokens.Accept<IVarKeywordToken>();
                // `ref` and `iref` are like a unary operator
                var expression = ParseExpression(OperatorPrecedence.Unary);
                var span = TextSpan.Covering(refToken.Span, expression.Span);
                return IRefExpressionSyntax.Create(span, isInternal, isVarBinding, expression);
            }
            case IAsyncKeywordToken _:
                return ParseAsyncBlock();
            case IDoKeywordToken _:
                return ParseAsyncStartExpression(scheduled: false);
            case IGoKeywordToken _:
                return ParseAsyncStartExpression(scheduled: true);
            case IAwaitKeywordToken _:
                return ParseAwaitExpression();
            case IOpenBraceToken _:
                return ParseBlock();
            case IBinaryOperatorToken _:
            case IAssignmentOperatorToken _:
            case IQuestionDotToken _:
            case ISemicolonToken _:
            case ICloseParenToken _:
                // If it is one of these, we assume there is a missing identifier
                return ParseMissingNameExpression();
            case IRightDoubleArrowToken _:
                throw new NotImplementedException($"`{Tokens.Current.Text(File.Code)}` in expression position");
        }
    }

    private ISelfExpressionSyntax ParseSelfExpression()
    {
        var selfKeyword = Tokens.Consume<ISelfKeywordToken>();
        return ISelfExpressionSyntax.Create(selfKeyword, false);
    }

    private IMissingNameExpressionSyntax ParseMissingNameExpression()
    {
        var identifierSpan = Tokens.Expect<IIdentifierToken>();
        return IMissingNameExpressionSyntax.Create(identifierSpan);
    }

    private IUnsafeExpressionSyntax ParseUnsafeExpression()
    {
        var unsafeKeyword = Tokens.Consume<IUnsafeKeywordToken>();
        var isBlock = Tokens.Current is IOpenBraceToken;
        var expression = isBlock
            ? ParseBlock()
            : ParseParenthesizedExpression();
        var span = TextSpan.Covering(unsafeKeyword, expression.Span);
        return IUnsafeExpressionSyntax.Create(span, expression);
    }

    private IUnaryOperatorExpressionSyntax ParsePrefixUnaryOperator(UnaryOperator @operator)
    {
        var operatorSpan = Tokens.Consume<IOperatorToken>();
        var operand = ParseExpression(OperatorPrecedence.Unary);
        var span = TextSpan.Covering(operatorSpan, operand.Span);
        return IUnaryOperatorExpressionSyntax.Create(span, UnaryOperatorFixity.Prefix, @operator, operand);
    }

    private IAsyncStartExpressionSyntax ParseAsyncStartExpression(bool scheduled)
    {
        var operatorSpan = Tokens.Consume<IAsyncStartOperatorToken>();
        var expression = ParseExpression(OperatorPrecedence.Min);
        var span = TextSpan.Covering(operatorSpan, expression.Span);
        return IAsyncStartExpressionSyntax.Create(span, scheduled, expression);
    }

    private IAwaitExpressionSyntax ParseAwaitExpression()
    {
        var awaitKeyword = Tokens.Expect<IAwaitKeywordToken>();
        var expression = ParseExpression(OperatorPrecedence.Unary);
        var span = TextSpan.Covering(awaitKeyword, expression.Span);
        return IAwaitExpressionSyntax.Create(span, expression);
    }

    private IForeachExpressionSyntax ParseForeach()
    {
        var foreachKeyword = Tokens.Consume<IForeachKeywordToken>();
        var mutableBinding = Tokens.Accept<IVarKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var variableName = identifier.Value;
        var nameSpan = identifier.Span;
        ITypeSyntax? type = null;
        if (Tokens.Accept<IColonToken>())
            type = ParseType();
        Tokens.Expect<IInKeywordToken>();
        var expression = ParseExpression();
        var block = ParseBlock();
        var span = TextSpan.Covering(foreachKeyword, block.Span);
        return IForeachExpressionSyntax.Create(span, mutableBinding, nameSpan, variableName, expression, type, block);
    }

    private IWhileExpressionSyntax ParseWhile()
    {
        var whileKeyword = Tokens.Consume<IWhileKeywordToken>();
        var condition = ParseExpression();
        var block = ParseBlock();
        var span = TextSpan.Covering(whileKeyword, block.Span);
        return IWhileExpressionSyntax.Create(span, condition, block);
    }

    private ILoopExpressionSyntax ParseLoop()
    {
        var loopKeyword = Tokens.Consume<ILoopKeywordToken>();
        var block = ParseBlock();
        var span = TextSpan.Covering(loopKeyword, block.Span);
        return ILoopExpressionSyntax.Create(span, block);
    }

    private IIfExpressionSyntax ParseIf(ParseAs parseAs = ParseAs.Expression)
    {
        var @if = Tokens.Consume<IIfKeywordToken>();
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
        return IIfExpressionSyntax.Create(span, condition, thenBlock, elseClause);
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

    private IExpressionSyntax ParseAsyncBlock()
    {
        var async = Tokens.Consume<IAsyncKeywordToken>();
        var block = ParseBlock();
        var span = TextSpan.Covering(async, block.Span);
        return IAsyncBlockExpressionSyntax.Create(span, block);
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

    public IFixedList<IExpressionSyntax> ParseArguments()
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
        return IResultStatementSyntax.Create(span, expression);
    }
}
