using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Attribute = Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree.Attribute;

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
        var declarations = BuildNonMemberDeclarations(packageSyntax.EntityDeclarations, FixedSet<INonMemberDeclaration>.Empty);
        var testingDeclarations = BuildNonMemberDeclarations(packageSyntax.TestingEntityDeclarations, declarations);

        var symbolTree = packageSyntax.SymbolTree.Build();
        var testingSymbolTree = packageSyntax.TestingSymbolTree.Build();
        return new PackageBuilder(declarations, testingDeclarations, symbolTree, testingSymbolTree,
            packageSyntax.Diagnostics, packageSyntax.References);
    }

    private static FixedSet<INonMemberDeclaration> BuildNonMemberDeclarations(
        FixedSet<IEntityDeclarationSyntax> entityDeclarations,
        FixedSet<INonMemberDeclaration> existingDeclarations)
    {
        var declarationDictionary = existingDeclarations.ToDictionary(d => d.Symbol, d => new Lazy<INonMemberDeclaration>(d));
        foreach (var declaration in entityDeclarations.OfType<INonMemberEntityDeclarationSyntax>())
        {
            declarationDictionary.Add(declaration.Symbol.Result,
                new(() => BuildNonMemberDeclaration(declaration, declarationDictionary)));
        }

        var declarations = declarationDictionary.Values.Select(l => l.Value).Except(existingDeclarations).ToFixedSet();
        return declarations;
    }

    private static INonMemberDeclaration BuildNonMemberDeclaration(
        INonMemberEntityDeclarationSyntax entity,
        IReadOnlyDictionary<Symbol, Lazy<INonMemberDeclaration>> declarations)
    {
        return entity switch
        {
            IClassDeclarationSyntax syn => BuildClass(syn, declarations),
            ITraitDeclarationSyntax syn => BuildTrait(syn, declarations),
            IFunctionDeclarationSyntax syn => BuildFunction(syn),
            _ => throw ExhaustiveMatch.Failed(entity)
        };
    }

    private static IClassDeclaration BuildClass(
        IClassDeclarationSyntax syn,
        IReadOnlyDictionary<Symbol, Lazy<INonMemberDeclaration>> declarations)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var baseClassSymbol = syn.BaseTypeName?.ReferencedSymbol.Result;
        var baseClass = baseClassSymbol is not null ? (IClassDeclaration)declarations[baseClassSymbol].Value : null;
        var supertypes = syn.SupertypeNames.Select(n => (ITypeDeclaration)declarations[n.ReferencedSymbol.Result!].Value).ToFixedList();
        var defaultConstructorSymbol = syn.DefaultConstructorSymbol;
        return new ClassDeclaration(syn.File, syn.Span, symbol, nameSpan, baseClass, supertypes, defaultConstructorSymbol, BuildMembers);

        FixedList<IClassMemberDeclaration> BuildMembers(IClassDeclaration c)
            => syn.Members.Select(m => BuildClassMember(c, m)).ToFixedList();
    }

    private static ITraitDeclaration BuildTrait(
        ITraitDeclarationSyntax syn,
        IReadOnlyDictionary<Symbol, Lazy<INonMemberDeclaration>> declarations)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var supertypes = syn.SupertypeNames.Select(n => (ITypeDeclaration)declarations[n.ReferencedSymbol.Result!].Value).ToFixedList();
        return new TraitDeclaration(syn.File, syn.Span, symbol, nameSpan, supertypes, BuildMembers);

        FixedList<ITraitMemberDeclaration> BuildMembers(ITraitDeclaration t)
            => syn.Members.Select(m => BuildTraitMember(t, m)).ToFixedList();
    }

    private static IClassMemberDeclaration BuildClassMember(
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

    private static ITraitMemberDeclaration BuildTraitMember(
        ITraitDeclaration declaringTrait,
        ITraitMemberDeclarationSyntax member)
    {
        return member switch
        {
            IAssociatedFunctionDeclarationSyntax syn => BuildAssociatedFunction(declaringTrait, syn),
            IAbstractMethodDeclarationSyntax syn => BuildAbstractMethod(declaringTrait, syn),
            IConcreteMethodDeclarationSyntax syn => BuildConcreteMethod(declaringTrait, syn),
            _ => throw ExhaustiveMatch.Failed(member)
        };
    }

    private static IAssociatedFunctionDeclaration BuildAssociatedFunction(
        ITypeDeclaration declaringType,
        IAssociatedFunctionDeclarationSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBody(syn.Body);
        return new AssociatedFunctionDeclaration(syn.File, syn.Span, declaringType, symbol, nameSpan, parameters, body);
    }

    private static IAbstractMethodDeclaration BuildAbstractMethod(
        ITypeDeclaration declaringType,
        IAbstractMethodDeclarationSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.SelfParameter);
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        return new AbstractMethodDeclaration(syn.File, syn.Span, declaringType, symbol, nameSpan, selfParameter, parameters);
    }

    private static IConcreteMethodDeclaration BuildConcreteMethod(
        ITypeDeclaration declaringType,
        IConcreteMethodDeclarationSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.SelfParameter);
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBody(syn.Body);
        return new ConcreteMethodDeclaration(syn.File, syn.Span, declaringType, symbol, nameSpan, selfParameter, parameters, body);
    }

    private static IConstructorDeclaration BuildConstructor(
        IClassDeclaration declaringClass,
        IConstructorDeclarationSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var selfParameter = BuildParameter(syn.SelfParameter);
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBlockBody(syn.Body);
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
        var attributes = BuildAttributes(syn.Attributes);
        var symbol = syn.Symbol.Result;
        var nameSpan = syn.NameSpan;
        var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
        var body = BuildBody(syn.Body);
        return new FunctionDeclaration(syn.File, syn.Span, attributes, symbol, nameSpan, parameters, body);
    }

    private static FixedList<IAttribute> BuildAttributes(IEnumerable<IAttributeSyntax> syn)
        => syn.Select(BuildAttribute).ToFixedList();

    private static IAttribute BuildAttribute(IAttributeSyntax syn)
    {
        var referencedSymbol = syn.TypeName.ReferencedSymbol.Result.Assigned();
        return new Attribute(syn.Span, referencedSymbol);
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
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
        var defaultValue = BuildExpression(syn.DefaultValue);
        return new FieldParameter(syn.Span, referencedSymbol, defaultValue);
    }

    private static IBody BuildBody(IBodySyntax syn)
    {
        return syn switch
        {
            IBlockBodySyntax body => BuildBlockBody(body),
            IExpressionBodySyntax body => BuildExpressionBody(body),
            _ => throw ExhaustiveMatch.Failed(syn),
        };
    }

    private static IBody BuildBlockBody(IBlockBodySyntax syn)
    {
        var statements = syn.Statements.Select(BuildBodyStatement).ToFixedList();
        return new Body(syn.Span, statements);
    }

    private static IBody BuildExpressionBody(IExpressionBodySyntax syn)
    {
        var exp = BuildExpression(syn.ResultStatement.Expression);
        var returnExp = new ReturnExpression(syn.ResultStatement.Span, exp.DataType, exp.Semantics, exp);
        var returnStmt = new ExpressionStatement(syn.ResultStatement.Span, returnExp);
        return new Body(syn.Span, returnStmt.Yield().ToFixedList<IBodyStatement>());
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

    [return: NotNullIfNotNull(nameof(expressionSyntax))]
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
            IPatternMatchExpressionSyntax syn => BuildPatternMatchExpression(syn),
            IAsyncBlockExpressionSyntax syn => BuildAsyncBlockExpression(syn),
            IAsyncStartExpressionSyntax syn => BuildAsyncStartExpression(syn),
            IAwaitExpressionSyntax syn => BuildAwaitExpression(syn),
            _ => throw ExhaustiveMatch.Failed(expressionSyntax),
        };
        return BuildImplicitConversion(expression, expressionSyntax);
    }

    private static IExpression BuildImplicitConversion(IExpression expression, IExpressionSyntax expressionSyntax)
    {
        if (expressionSyntax.ImplicitConversion is Conversion conversion)
            return BuildImplicitConversion(expression, conversion);

        return expression;
    }

    private static IExpression BuildImplicitConversion(IExpression expression, Conversion conversion)
    {
        if (conversion is ChainedConversion chainedConversion)
            expression = BuildImplicitConversion(expression, chainedConversion.PriorConversion);
        return conversion switch
        {
            LiftedConversion _ => BuildImplicitLiftedConversionExpression(expression, conversion),
            NumericConversion _ => BuildImplicitNumericConversionExpression(expression, conversion),
            OptionalConversion _ => BuildImplicitOptionalConversionExpression(expression, conversion),
            RecoverIsolation _ => BuildRecoverIsolationExpression(expression, conversion),
            RecoverConst _ => BuildRecoverConstExpression(expression, conversion),
            ImplicitMove _ => BuildImplicitMoveExpression(expression, conversion),
            ImplicitFreeze _ => BuildImplicitFreezeExpression(expression, conversion),
            IdentityConversion _ => expression,
            _ => throw ExhaustiveMatch.Failed(conversion)
        };
    }

    private static FixedList<IExpression> BuildExpressions(FixedList<IExpressionSyntax> expressions)
        // The compiler isn't able to correctly figure out the nullability here. That is actually
        // why this method even exists
        => expressions.Select(BuildExpression).ToFixedList()!;

    private static IAssignmentExpression BuildAssignmentExpression(IAssignmentExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
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
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var leftOperand = BuildExpression(syn.LeftOperand);
        var @operator = syn.Operator;
        var rightOperand = BuildExpression(syn.RightOperand);
        return new BinaryOperatorExpression(syn.Span, type, semantics, leftOperand, @operator, rightOperand);
    }

    private static IBlockExpression BuildBlockExpression(IBlockExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var statements = syn.Statements.Select(BuildStatement).ToFixedList();
        return new BlockExpression(syn.Span, type, semantics, statements);
    }

    private static IBoolLiteralExpression BuildBoolLiteralExpression(IBoolLiteralExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var value = syn.Value;
        return new BoolLiteralExpression(syn.Span, type, semantics, value);
    }

    private static IBreakExpression BuildBreakExpression(IBreakExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var value = BuildExpression(syn.Value);
        return new BreakExpression(syn.Span, type, semantics, value);
    }

    private static IFieldAccessExpression BuildFieldAccessExpression(IQualifiedNameExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var context = BuildExpression(syn.Context);
        var accessOperator = syn.AccessOperator;
        var referencedSymbol = (FieldSymbol)syn.ReferencedSymbol.Result.Assigned();
        return new FieldAccessExpression(syn.Span, type, semantics, context, accessOperator, referencedSymbol);
    }

    private static IForeachExpression BuildForeachExpression(IForeachExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var symbol = syn.Symbol.Result;
        var inExpression = BuildExpression(syn.InExpression);
        var block = BuildBlockExpression(syn.Block);
        return new ForeachExpression(syn.Span, type, semantics, symbol, inExpression, block);
    }

    private static IInvocationExpression BuildInvocationExpression(
        IInvocationExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
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
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var condition = BuildExpression(syn.Condition);
        var thenBlock = BuildBlockOrResult(syn.ThenBlock);
        var elseClause = BuildElseClause(syn.ElseClause);
        return new IfExpression(syn.Span, type, semantics, condition, thenBlock, elseClause);
    }

    [return: NotNullIfNotNull(nameof(elseClause))]
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
        Conversion conversion)
    {
        var (convertToType, semantics) = conversion.Apply(expression.DataType, expression.Semantics);
        return new ImplicitLiftedConversion(expression.Span, convertToType, semantics, expression, (OptionalType)convertToType);
    }

    private static IImplicitNumericConversionExpression BuildImplicitNumericConversionExpression(
        IExpression expression,
        Conversion conversion)
    {
        var (convertToType, semantics) = conversion.Apply(expression.DataType, expression.Semantics);
        return new ImplicitNumericConversionExpression(expression.Span, semantics, expression, (NumericType)convertToType);
    }

    private static IImplicitOptionalConversionExpression BuildImplicitOptionalConversionExpression(
        IExpression expression,
        Conversion conversion)
    {
        var (convertToType, semantics) = conversion.Apply(expression.DataType, expression.Semantics);
        return new ImplicitOptionalConversionExpression(expression.Span, convertToType, semantics, expression, (OptionalType)convertToType);
    }

    private static IRecoverIsolationExpression BuildRecoverIsolationExpression(
        IExpression expression,
        Conversion conversion)
    {
        var (convertToType, semantics) = conversion.Apply(expression.DataType, expression.Semantics);
        return new RecoverIsolationExpression(expression.Span, (ReferenceType)convertToType, semantics, expression);
    }

    private static IRecoverConstExpression BuildRecoverConstExpression(
        IExpression expression,
        Conversion conversion)
    {
        var (convertToType, semantics) = conversion.Apply(expression.DataType, expression.Semantics);
        return new RecoverConstExpression(expression.Span, (ReferenceType)convertToType, semantics, expression);
    }

    private static IIntegerLiteralExpression BuildIntegerLiteralExpression(IIntegerLiteralExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var value = syn.Value;
        return new IntegerLiteralExpression(syn.Span, type, semantics, value);
    }

    private static INoneLiteralExpression BuildNoneLiteralExpression(INoneLiteralExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        return new NoneLiteralExpression(syn.Span, type, semantics);
    }

    private static IStringLiteralExpression BuildStringLiteralExpression(IStringLiteralExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var value = syn.Value;
        return new StringLiteralExpression(syn.Span, type, semantics, value);
    }

    private static ILoopExpression BuildLoopExpression(ILoopExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var block = BuildBlockExpression(syn.Block);
        return new LoopExpression(syn.Span, type, semantics, block);
    }

    private static IMoveExpression BuildMoveExpression(IMoveExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
        var referent = BuildExpression(syn.Referent);
        return new MoveExpression(syn.Span, type, semantics, referencedSymbol, referent);
    }

    private static IMoveExpression BuildImplicitMoveExpression(
        IExpression expression,
        Conversion conversion)
    {
        var (convertToType, semantics) = conversion.Apply(expression.DataType, expression.Semantics);
        var referencedSymbol = ((INameExpression)expression).ReferencedSymbol;
        return new MoveExpression(expression.Span, (ReferenceType)convertToType, semantics, referencedSymbol, expression);
    }

    private static INameExpression BuildNameExpression(ISimpleNameExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = (NamedBindingSymbol)syn.ReferencedSymbol.Result.Assigned();
        return new NameExpression(syn.Span, type, semantics, referencedSymbol);
    }

    private static INewObjectExpression BuildNewObjectExpression(INewObjectExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
        var arguments = BuildExpressions(syn.Arguments);
        return new NewObjectExpression(syn.Span, type, semantics, referencedSymbol, arguments);
    }

    private static INextExpression BuildNextExpression(INextExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        return new NextExpression(syn.Span, type, semantics);
    }

    private static IReturnExpression BuildReturnExpression(IReturnExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var value = BuildExpression(syn.Value);
        return new ReturnExpression(syn.Span, type, semantics, value);
    }

    private static ISelfExpression BuildSelfExpression(ISelfExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
        var isImplicit = syn.IsImplicit;
        return new SelfExpression(syn.Span, type, semantics, referencedSymbol, isImplicit);
    }

    private static IUnaryOperatorExpression BuildUnaryOperatorExpression(IUnaryOperatorExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var fixity = syn.Fixity;
        var @operator = syn.Operator;
        var operand = BuildExpression(syn.Operand);
        return new UnaryOperatorExpression(syn.Span, type, semantics, fixity, @operator, operand);
    }

    private static IUnsafeExpression BuildUnsafeExpression(IUnsafeExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var expression = BuildExpression(syn.Expression);
        return new UnsafeExpression(syn.Span, type, semantics, expression);
    }

    private static IWhileExpression BuildWhileExpression(IWhileExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var condition = BuildExpression(syn.Condition);
        var block = BuildBlockExpression(syn.Block);
        return new WhileExpression(syn.Span, type, semantics, condition, block);
    }

    private static IExpression BuildIdExpression(IIdExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var referent = BuildExpression(syn.Referent);
        return new IdExpression(syn.Span, type, semantics, referent);
    }

    private static IFreezeExpression BuildFreezeExpression(IFreezeExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var referencedSymbol = syn.ReferencedSymbol.Result.Assigned();
        var referent = BuildExpression(syn.Referent);
        return new FreezeExpression(syn.Span, type, semantics, referencedSymbol, referent);
    }

    private static IFreezeExpression BuildImplicitFreezeExpression(
        IExpression expression,
        Conversion conversion)
    {
        var (convertToType, semantics) = conversion.Apply(expression.DataType, expression.Semantics);
        var referencedSymbol = ((INameExpression)expression).ReferencedSymbol;
        return new FreezeExpression(expression.Span, (ReferenceType)convertToType, semantics, referencedSymbol, expression);
    }

    private static IExpression BuildConversionExpression(IConversionExpressionSyntax syn)
    {
        var referent = BuildExpression(syn.Referent);
        if (syn.Operator == ConversionOperator.Safe)
            return BuildImplicitNumericConversionExpression(referent, syn);

        // TODO support non-numeric conversions
        var semantics = syn.ConvertedSemantics!.Value;
        var expressionType = syn.DataType.Assigned();
        var isOptional = syn.Operator == ConversionOperator.Optional;
        var convertToType = (NumericType)syn.ConvertToType.NamedType.Assigned();
        return new ExplicitNumericConversion(syn.Span, expressionType, semantics, referent, isOptional, convertToType);
    }

    private static IExpression BuildImplicitNumericConversionExpression(
        IExpression expression, IConversionExpressionSyntax conversion)
    {
        var convertToType = (NumericType)conversion.ConvertToType.NamedType.Assigned();
        var semantics = conversion.ConvertedSemantics.Assigned();
        return new ImplicitNumericConversionExpression(expression.Span, semantics, expression, convertToType);
    }

    private static IExpression BuildPatternMatchExpression(IPatternMatchExpressionSyntax syn)
    {
        var referent = BuildExpression(syn.Referent);
        var pattern = BuildPattern(syn.Pattern);
        return new PatternMatchExpression(referent, pattern);
    }

    private static IPattern BuildPattern(IPatternSyntax patternSyntax)
    {
        return patternSyntax switch
        {
            IBindingContextPatternSyntax syn => BuildBindingContextPattern(syn),
            IBindingPatternSyntax syn => BuildBindingPattern(syn),
            IOptionalPatternSyntax syn => BuildOptionalPattern(syn),
            _ => throw ExhaustiveMatch.Failed(patternSyntax),
        };
    }

    private static IPattern BuildBindingContextPattern(IBindingContextPatternSyntax syn)
    {
        var pattern = BuildPattern(syn.Pattern);
        if (syn.Type is null) return pattern;
        var type = syn.Type.NamedType.Assigned();
        return new BindingContextPattern(syn.Span, pattern, type);
    }

    private static IBindingPattern BuildBindingPattern(IBindingPatternSyntax syn)
    {
        var symbol = syn.Symbol.Result;
        return new BindingPattern(syn.Span, symbol);
    }

    private static IOptionalPattern BuildOptionalPattern(IOptionalPatternSyntax syn)
    {
        var pattern = BuildPattern(syn.Pattern);
        return new OptionalPattern(syn.Span, pattern);
    }

    private static IAsyncBlockExpression BuildAsyncBlockExpression(IAsyncBlockExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var block = BuildBlockExpression(syn.Block);
        return new AsyncBlockExpression(syn.Span, type, semantics, block);
    }

    private static IAsyncStartExpression BuildAsyncStartExpression(IAsyncStartExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var expression = BuildExpression(syn.Expression);
        return new AsyncStartExpression(syn.Span, type, semantics, syn.Scheduled, expression);
    }

    private static IAwaitExpression BuildAwaitExpression(IAwaitExpressionSyntax syn)
    {
        var type = syn.DataType.Assigned();
        var semantics = syn.Semantics.Assigned();
        var expression = BuildExpression(syn.Expression);
        return new AwaitExpression(syn.Span, type, semantics, expression);
    }
}
