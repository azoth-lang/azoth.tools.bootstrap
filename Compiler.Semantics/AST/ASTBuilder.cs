using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST;

// ReSharper disable once UnusedMember.Global
[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes",
    Justification = "In Progress")]
internal class ASTBuilder
{
    // ReSharper disable once UnusedMember.Global
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public PackageBuilder BuildPackage(PackageSyntax<Package> packageSyntax)
    {
        var nonMemberDeclarations = packageSyntax.AllEntityDeclarations
                                                 .OfType<INonMemberEntityDeclarationSyntax>()
                                                 .Select(BuildNonMemberDeclaration).ToFixedList();

        var symbolTree = packageSyntax.SymbolTree.Build();
        return new PackageBuilder(nonMemberDeclarations, symbolTree, packageSyntax.Diagnostics, packageSyntax.References);
    }

    private static INonMemberDeclaration BuildNonMemberDeclaration(INonMemberEntityDeclarationSyntax entity)
    {
        return entity switch
        {
            IClassDeclarationSyntax syn => BuildClass(syn),
            IFunctionDeclarationSyntax syn => BuildFunction(syn),
            _ => throw ExhaustiveMatch.Failed(entity)
        };
    }

    private static IClassDeclaration BuildClass(IClassDeclarationSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var defaultConstructorSymbol = syn.DefaultConstructorSymbol;
        return new ClassDeclaration(syn.File, syn.Span, symbol, nameSpan, defaultConstructorSymbol, BuildMembers);

        FixedList<IMemberDeclaration> BuildMembers(IClassDeclaration c)
            => syn.Members.Select(m => BuildMember(c, m)).ToFixedList();
    }

    private static IMemberDeclaration BuildMember(
        IClassDeclaration declaringClass,
        IMemberDeclarationSyntax member)
    {
        return member switch
        {
            IAssociatedFunctionDeclarationSyntax syn => BuildAssociatedFunction(declaringClass, syn),
            IAbstractMethodDeclarationSyntax syn => BuildAbstractMethod(declaringClass, syn),
            IConcreteMethodDeclarationSyntax syn => BuildConcreteMethod(declaringClass, syn),
            IConstructorDeclarationSyntax syn => BuildConstructor(declaringClass, syn),
            IFieldDeclarationSyntax syn => BuildField(declaringClass, syn),
            _ => throw ExhaustiveMatch.Failed(member)
        };
    }

    private static IAssociatedFunctionDeclaration BuildAssociatedFunction(
        IClassDeclaration declaringClass,
        IAssociatedFunctionDeclarationSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBody(syn.Body);
        return new AssociatedFunctionDeclaration(syn.File, syn.Span, declaringClass, symbol, nameSpan, parameters, body);
    }

    private static IAbstractMethodDeclaration BuildAbstractMethod(
        IClassDeclaration declaringClass,
        IAbstractMethodDeclarationSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.SelfParameter);
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        return new AbstractMethodDeclaration(syn.File, syn.Span, declaringClass, symbol, nameSpan, selfParameter, parameters);
    }

    private static IConcreteMethodDeclaration BuildConcreteMethod(
        IClassDeclaration declaringClass,
        IConcreteMethodDeclarationSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.SelfParameter);
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBody(syn.Body);
        return new ConcreteMethodDeclaration(syn.File, syn.Span, declaringClass, symbol, nameSpan, selfParameter, parameters, body);
    }

    private static IConstructorDeclaration BuildConstructor(
        IClassDeclaration declaringClass,
        IConstructorDeclarationSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.ImplicitSelfParameter);
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBody(syn.Body);
        return new ConstructorDeclaration(syn.File, syn.Span, declaringClass, symbol, nameSpan, selfParameter, parameters, body);
    }

    private static IFieldDeclaration BuildField(
        IClassDeclaration declaringClass,
        IFieldDeclarationSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        return new FieldDeclaration(syn.File, syn.Span, declaringClass, symbol, nameSpan);
    }

    private static IFunctionDeclaration BuildFunction(IFunctionDeclarationSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBody(syn.Body);
        return new FunctionDeclaration(syn.File, syn.Span, symbol, nameSpan, parameters, body);
    }

    private static IConstructorParameter BuildParameter(IConstructorParameterSyntax parameter)
    {
        return parameter switch
        {
            INamedParameterSyntax syn => BuildParameter(syn),
            IFieldParameterSyntax syn => BuildParameter(syn),
            _ => throw ExhaustiveMatch.Failed(parameter),
        };
    }

    private static INamedParameter BuildParameter(INamedParameterSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var defaultValue = BuildExpression(syn.DefaultValue);
        return new NamedParameter(syn.Span, symbol, syn.Unused, defaultValue);
    }

    private static ISelfParameter BuildParameter(ISelfParameterSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var unused = syn.Unused;
        return new SelfParameter(syn.Span, symbol, unused);
    }

    private static IFieldParameter BuildParameter(IFieldParameterSyntax syn)
    {
        var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
        var defaultValue = BuildExpression(syn.DefaultValue);
        return new FieldParameter(syn.Span, referencedSymbol, defaultValue);
    }

    private static IBody BuildBody(IBodySyntax syn)
    {
        var statements = syn.Statements.Select(BuildBodyStatement).ToFixedList();
        return new Body(syn.Span, statements);
    }

    private static IStatement BuildStatement(IStatementSyntax stmt)
    {
        return stmt switch
        {
            IResultStatementSyntax syn => BuildResultStatement(syn),
            IBodyStatementSyntax syn => BuildBodyStatement(syn),
            _ => throw ExhaustiveMatch.Failed(stmt),
        };
    }

    private static IBodyStatement BuildBodyStatement(IBodyStatementSyntax stmt)
    {
        return stmt switch
        {
            IExpressionStatementSyntax syn => BuildExpressionStatement(syn),
            IVariableDeclarationStatementSyntax syn => BuildVariableDeclaration(syn),
            _ => throw ExhaustiveMatch.Failed(stmt),
        };
    }

    private static IExpressionStatement BuildExpressionStatement(IExpressionStatementSyntax syn)
    {
        var expression = BuildExpression(syn.Expression);
        return new ExpressionStatement(syn.Span, expression);
    }

    private static IVariableDeclarationStatement BuildVariableDeclaration(IVariableDeclarationStatementSyntax syn)
    {
        var nameSpan = syn.NameSpan;
        var symbol = syn.Symbol.Result;
        var initializer = BuildExpression(syn.Initializer);
        return new VariableDeclarationStatement(syn.Span, nameSpan, symbol, initializer);
    }

    private static IResultStatement BuildResultStatement(IResultStatementSyntax syn)
    {
        var expression = BuildExpression(syn.Expression);
        return new ResultStatement(syn.Span, expression);
    }

    private static IBlockOrResult BuildBlockOrResult(IBlockOrResultSyntax syntax)
    {
        return syntax switch
        {
            IBlockExpressionSyntax syn => BuildBlockExpression(syn),
            IResultStatementSyntax syn => BuildResultStatement(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };
    }

    [return: NotNullIfNotNull("expressionSyntax")]
    private static IExpression? BuildExpression(IExpressionSyntax? expressionSyntax)
    {
        if (expressionSyntax is null) return null;
        IExpression expression = expressionSyntax switch
        {
            IAssignmentExpressionSyntax syn => BuildAssignmentExpression(syn),
            IBinaryOperatorExpressionSyntax syn => BuildBinaryOperatorExpression(syn),
            IBlockExpressionSyntax syn => BuildBlockExpression(syn),
            IBoolLiteralExpressionSyntax syn => BuildBoolLiteralExpression(syn),
            IBreakExpressionSyntax syn => BuildBreakExpression(syn),
            IQualifiedNameExpressionSyntax syn => BuildFieldAccessExpression(syn),
            IForeachExpressionSyntax syn => BuildForeachExpression(syn),
            IIfExpressionSyntax syn => BuildIfExpression(syn),
            IIntegerLiteralExpressionSyntax syn => BuildIntegerLiteralExpression(syn),
            INoneLiteralExpressionSyntax syn => BuildNoneLiteralExpression(syn),
            IStringLiteralExpressionSyntax syn => BuildStringLiteralExpression(syn),
            ILoopExpressionSyntax syn => BuildLoopExpression(syn),
            IMoveExpressionSyntax syn => BuildMoveExpression(syn),
            ISimpleNameExpressionSyntax syn => BuildNameExpression(syn),
            INewObjectExpressionSyntax syn => BuildNewObjectExpression(syn),
            IInvocationExpressionSyntax syn => BuildInvocationExpression(syn),
            INextExpressionSyntax syn => BuildNextExpression(syn),
            IReturnExpressionSyntax syn => BuildReturnExpression(syn),
            ISelfExpressionSyntax syn => BuildSelfExpression(syn),
            IUnaryOperatorExpressionSyntax syn => BuildUnaryOperatorExpression(syn),
            IUnsafeExpressionSyntax syn => BuildUnsafeExpression(syn),
            IWhileExpressionSyntax syn => BuildWhileExpression(syn),
            IIdExpressionSyntax syn => BuildIdExpression(syn),
            IFreezeExpressionSyntax syn => BuildFreezeExpression(syn),
            IConversionExpressionSyntax syn => BuildConversionExpression(syn),
            _ => throw ExhaustiveMatch.Failed(expressionSyntax),
        };
        return BuildImplicitConversion(expression, expressionSyntax);
    }

    private static IExpression BuildImplicitConversion(IExpression expression, IExpressionSyntax expressionSyntax)
    {
        return expressionSyntax.ImplicitConversion switch
        {
            null => expression,
            LiftedConversion _ => BuildImplicitLiftedConversionExpression(expression, expressionSyntax),
            NumericConversion _ => BuildImplicitNumericConversionExpression(expression, expressionSyntax),
            OptionalConversion _ => BuildImplicitOptionalConversionExpression(expression, expressionSyntax),
            RecoverIsolation _ => BuildRecoverIsolationExpression(expression, expressionSyntax),
            RecoverConst _ => BuildRecoverConstExpression(expression, expressionSyntax),
            ImplicitMove _ => BuildImplicitMoveExpression(expression, expressionSyntax),
            ImplicitFreeze _ => BuildImplicitFreezeExpression(expression, expressionSyntax),
            IdentityConversion _ => expression,
            _ => throw ExhaustiveMatch.Failed(expressionSyntax.ImplicitConversion)
        };
    }

    private static FixedList<IExpression> BuildExpressions(FixedList<IExpressionSyntax> expressions)
        // The compiler isn't able to correctly figure out the nullability here. That is actually
        // why this method even exists
        => expressions.Select(BuildExpression).ToFixedList()!;

    private static IAssignmentExpression BuildAssignmentExpression(IAssignmentExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var leftOperand = BuildAssignableExpression(syn.LeftOperand);
        var @operator = syn.Operator;
        var rightOperand = BuildExpression(syn.RightOperand);
        return new AssignmentExpression(syn.Span, type, semantics, leftOperand, @operator, rightOperand);
    }

    private static IAssignableExpression BuildAssignableExpression(IAssignableExpressionSyntax expression)
    {
        return expression switch
        {
            IQualifiedNameExpressionSyntax syn => BuildFieldAccessExpression(syn),
            ISimpleNameExpressionSyntax syn => BuildNameExpression(syn),
            _ => throw ExhaustiveMatch.Failed(expression),
        };
    }

    private static IBinaryOperatorExpression BuildBinaryOperatorExpression(IBinaryOperatorExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var leftOperand = BuildExpression(syn.LeftOperand);
        var @operator = syn.Operator;
        var rightOperand = BuildExpression(syn.RightOperand);
        return new BinaryOperatorExpression(syn.Span, type, semantics, leftOperand, @operator, rightOperand);
    }

    private static IBlockExpression BuildBlockExpression(IBlockExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var statements = syn.Statements.Select(BuildStatement).ToFixedList();
        return new BlockExpression(syn.Span, type, semantics, statements);
    }

    private static IBoolLiteralExpression BuildBoolLiteralExpression(IBoolLiteralExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var value = syn.Value;
        return new BoolLiteralExpression(syn.Span, type, semantics, value);
    }

    private static IBreakExpression BuildBreakExpression(IBreakExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var value = BuildExpression(syn.Value);
        return new BreakExpression(syn.Span, type, semantics, value);
    }

    private static IFieldAccessExpression BuildFieldAccessExpression(IQualifiedNameExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var context = BuildExpression(syn.Context);
        var accessOperator = syn.AccessOperator;
        var referencedSymbol = (FieldSymbol?)syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
        return new FieldAccessExpression(syn.Span, type, semantics, context, accessOperator, referencedSymbol);
    }

    private static IForeachExpression BuildForeachExpression(IForeachExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var symbol = syn.Symbol.Result;
        var inExpression = BuildExpression(syn.InExpression);
        var block = BuildBlockExpression(syn.Block);
        return new ForeachExpression(syn.Span, type, semantics, symbol, inExpression, block);
    }

    private static IInvocationExpression BuildInvocationExpression(
        IInvocationExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
        var arguments = BuildExpressions(syn.Arguments);
        switch (referencedSymbol)
        {
            default:
                throw ExhaustiveMatch.Failed(referencedSymbol);
            case FunctionSymbol function:
                return new FunctionInvocationExpression(syn.Span, type, semantics, function, arguments);
            case MethodSymbol method:
                var qualifiedName = (IQualifiedNameExpressionSyntax)syn.Expression;
                var context = BuildExpression(qualifiedName.Context);
                return new MethodInvocationExpression(syn.Span, type, semantics, context, method, arguments);
            case ConstructorSymbol _:
                throw new InvalidOperationException("Invocation expression cannot invoke a constructor");
        }
    }

    private static IIfExpression BuildIfExpression(IIfExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var condition = BuildExpression(syn.Condition);
        var thenBlock = BuildBlockOrResult(syn.ThenBlock);
        var elseClause = BuildElseClause(syn.ElseClause);
        return new IfExpression(syn.Span, type, semantics, condition, thenBlock, elseClause);
    }

    [return: NotNullIfNotNull("syn")]
    private static IElseClause? BuildElseClause(IElseClauseSyntax? elseClause)
    {
        return elseClause switch
        {
            null => null,
            IBlockOrResultSyntax syn => BuildBlockOrResult(syn),
            IIfExpressionSyntax syn => BuildIfExpression(syn),
            _ => throw ExhaustiveMatch.Failed(elseClause),
        };
    }

    private static IImplicitLiftedConversionExpression BuildImplicitLiftedConversionExpression(
        IExpression expression,
        IExpressionSyntax expressionSyntax)
    {
        var semantics = expressionSyntax.ConvertedSemantics!.Value;
        var convertToType = (OptionalType)expressionSyntax.ConvertedDataType!;
        return new ImplicitLiftedConversion(expression.Span, convertToType, semantics, expression, convertToType);
    }

    private static IImplicitNumericConversionExpression BuildImplicitNumericConversionExpression(
        IExpression expression,
        IExpressionSyntax expressionSyntax)
    {
        var semantics = expressionSyntax.ConvertedSemantics!.Value;
        var convertToType = (NumericType)expressionSyntax.ConvertedDataType!;
        return new ImplicitNumericConversionExpression(expression.Span, convertToType, semantics, expression, convertToType);
    }

    private static IImplicitOptionalConversionExpression BuildImplicitOptionalConversionExpression(
        IExpression expression,
        IExpressionSyntax expressionSyntax)
    {
        var semantics = expressionSyntax.ConvertedSemantics!.Value;
        var convertToType = (OptionalType)expressionSyntax.ConvertedDataType!;
        return new ImplicitOptionalConversionExpression(expression.Span, convertToType, semantics, expression, convertToType);
    }

    private static IRecoverIsolationExpression BuildRecoverIsolationExpression(
        IExpression expression,
        IExpressionSyntax expressionSyntax)
    {
        var semantics = expressionSyntax.ConvertedSemantics!.Value;
        var convertToType = (ReferenceType)expressionSyntax.ConvertedDataType!;
        return new RecoverIsolationExpression(expression.Span, convertToType, semantics, expression);
    }

    private static IRecoverConstExpression BuildRecoverConstExpression(
        IExpression expression,
        IExpressionSyntax expressionSyntax)
    {
        var semantics = expressionSyntax.ConvertedSemantics!.Value;
        var convertToType = (ReferenceType)expressionSyntax.ConvertedDataType!;
        return new RecoverConstExpression(expression.Span, convertToType, semantics, expression);
    }

    private static IIntegerLiteralExpression BuildIntegerLiteralExpression(IIntegerLiteralExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var value = syn.Value;
        return new IntegerLiteralExpression(syn.Span, type, semantics, value);
    }

    private static INoneLiteralExpression BuildNoneLiteralExpression(INoneLiteralExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        return new NoneLiteralExpression(syn.Span, type, semantics);
    }

    private static IStringLiteralExpression BuildStringLiteralExpression(IStringLiteralExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var value = syn.Value;
        return new StringLiteralExpression(syn.Span, type, semantics, value);
    }

    private static ILoopExpression BuildLoopExpression(ILoopExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var block = BuildBlockExpression(syn.Block);
        return new LoopExpression(syn.Span, type, semantics, block);
    }

    private static IMoveExpression BuildMoveExpression(IMoveExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
        var referent = BuildExpression(syn.Referent);
        return new MoveExpression(syn.Span, type, semantics, referencedSymbol, referent);
    }

    private static IMoveExpression BuildImplicitMoveExpression(
        IExpression expression,
        IExpressionSyntax expressionSyntax)
    {
        var semantics = expressionSyntax.ConvertedSemantics!.Value;
        var convertToType = (ReferenceType)expressionSyntax.ConvertedDataType!;
        var referencedSymbol = ((INameExpression)expression).ReferencedSymbol;
        return new MoveExpression(expression.Span, convertToType, semantics, referencedSymbol, expression);
    }

    private static INameExpression BuildNameExpression(ISimpleNameExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = (NamedBindingSymbol?)syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
        return new NameExpression(syn.Span, type, semantics, referencedSymbol);
    }

    private static INewObjectExpression BuildNewObjectExpression(INewObjectExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
        var arguments = BuildExpressions(syn.Arguments);
        return new NewObjectExpression(syn.Span, type, semantics, referencedSymbol, arguments);
    }

    private static INextExpression BuildNextExpression(INextExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        return new NextExpression(syn.Span, type, semantics);
    }

    private static IReturnExpression BuildReturnExpression(IReturnExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var value = BuildExpression(syn.Value);
        return new ReturnExpression(syn.Span, type, semantics, value);
    }

    private static ISelfExpression BuildSelfExpression(ISelfExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
        var isImplicit = syn.IsImplicit;
        return new SelfExpression(syn.Span, type, semantics, referencedSymbol, isImplicit);
    }

    private static IUnaryOperatorExpression BuildUnaryOperatorExpression(IUnaryOperatorExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var fixity = syn.Fixity;
        var @operator = syn.Operator;
        var operand = BuildExpression(syn.Operand);
        return new UnaryOperatorExpression(syn.Span, type, semantics, fixity, @operator, operand);
    }

    private static IUnsafeExpression BuildUnsafeExpression(IUnsafeExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var expression = BuildExpression(syn.Expression);
        return new UnsafeExpression(syn.Span, type, semantics, expression);
    }

    private static IWhileExpression BuildWhileExpression(IWhileExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var condition = BuildExpression(syn.Condition);
        var block = BuildBlockExpression(syn.Block);
        return new WhileExpression(syn.Span, type, semantics, condition, block);
    }

    private static IExpression BuildIdExpression(IIdExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var referent = BuildExpression(syn.Referent);
        return new IdExpression(syn.Span, type, semantics, referent);
    }

    private static IFreezeExpression BuildFreezeExpression(IFreezeExpressionSyntax syn)
    {
        var type = syn.DataType ?? throw new InvalidOperationException();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
        var referent = BuildExpression(syn.Referent);
        return new FreezeExpression(syn.Span, type, semantics, referencedSymbol, referent);
    }

    private static IFreezeExpression BuildImplicitFreezeExpression(
        IExpression expression,
        IExpressionSyntax expressionSyntax)
    {
        var semantics = expressionSyntax.ConvertedSemantics!.Value;
        var convertToType = (ReferenceType)expressionSyntax.ConvertedDataType!;
        var referencedSymbol = ((INameExpression)expression).ReferencedSymbol;
        return new FreezeExpression(expressionSyntax.Span, convertToType, semantics, referencedSymbol, expression);
    }

    private static IImplicitNumericConversionExpression BuildConversionExpression(IConversionExpressionSyntax syn)
    {
        // TODO replace hack with a proper implementation
        var referent = BuildExpression(syn.Referent);
        return BuildImplicitNumericConversionExpression(referent, syn);
    }
}
