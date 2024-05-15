using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

/// <summary>
/// Applies entity symbols and types created in the semantic tree to the old syntax tree approach.
/// </summary>
/// <remarks>This is an intermediate phase to entirely eliminating the old approach using properties
/// on the concrete syntax tree.</remarks>
internal class SemanticsApplier
{
    #region Special Parts
    private static void ElseClause(IElseClauseNode? node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case null:
                break;
            case IBlockOrResultNode n:
                BlockOrResult(n);
                break;
            case IIfExpressionNode n:
                IfExpression(n);
                break;
        }
    }

    private static void BlockOrResult(IBlockOrResultNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IResultStatementNode n:
                ResultStatement(n);
                break;
            case IBlockExpressionNode n:
                BlockExpression(n);
                break;
        }
    }
    #endregion

    #region Packages
    public static void Apply(IPackageNode package)
    {
        PackageFacet(package.MainFacet);
        PackageFacet(package.TestingFacet);
    }

    private static void PackageFacet(IPackageFacetNode node)
        => node.CompilationUnits.ForEach(CompilationUnit);
    #endregion

    #region Code Files
    private static void CompilationUnit(ICompilationUnitNode node)
        => Declarations(node.Declarations);
    #endregion

    #region Declarations
    private static void Declarations(IEnumerable<IDeclarationNode> nodes)
        => nodes.ForEach(Declaration);

    private static void Declaration(IDeclarationNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case ITypeDeclarationNode n:
                TypeDeclaration(n);
                break;
            case IFunctionDeclarationNode n:
                FunctionDeclaration(n);
                break;
            case INamespaceDeclarationNode n:
                NamespaceDeclaration(n);
                break;
            case IMethodDeclarationNode n:
                MethodDeclaration(n);
                break;
            case IConstructorDeclarationNode n:
                ConstructorDeclaration(n);
                break;
            case IInitializerDeclarationNode n:
                InitializerDeclaration(n);
                break;
            case IFieldDeclarationNode n:
                FieldDeclaration(n);
                break;
            case IAssociatedFunctionDeclarationNode n:
                AssociatedFunctionDeclaration(n);
                break;
        }
    }
    #endregion

    #region Namespace Declarations
    private static void NamespaceDeclaration(INamespaceDeclarationNode node)
    {
        node.Syntax.Symbol.Fulfill(node.Symbol);
        Declarations(node.Declarations);
    }
    #endregion

    #region Function Declaration
    private static void FunctionDeclaration(IFunctionDeclarationNode node)
    {
        var syntax = node.Syntax;
        syntax.Symbol.BeginFulfilling();
        syntax.Symbol.Fulfill(node.Symbol);
        Attributes(node.Attributes);
        NamedParameters(node.Parameters);
        Type(node.Return);
        Body(node.Body);
    }
    #endregion

    #region Type Declarations
    private static void TypeDeclaration(ITypeDeclarationNode node)
    {
        var syntax = node.Syntax;
        syntax.Symbol.BeginFulfilling();
        syntax.Symbol.Fulfill(node.Symbol);
        GenericParameters(node.GenericParameters);
        StandardTypeNames(node.AllSupertypeNames);
        Declarations(node.Members);
    }
    #endregion

    #region Type Declaration Parts
    private static void GenericParameters(IFixedList<IGenericParameterNode> nodes)
        => nodes.ForEach(GenericParameter);

    private static void GenericParameter(IGenericParameterNode node)
        => node.Syntax.Symbol.Fulfill(node.Symbol);
    #endregion

    #region Member Declarations
    private static void MethodDeclaration(IMethodDeclarationNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        NamedParameters(node.Parameters);
        Type(node.Return);

        if (node is IConcreteMethodDeclarationNode n)
            ConcreteMethodDeclaration(n);
    }

    private static void ConcreteMethodDeclaration(IConcreteMethodDeclarationNode node)
        => Body(node.Body);

    private static void ConstructorDeclaration(IConstructorDeclarationNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        ConstructorOrInitializerParameters(node.Parameters);
        BlockBody(node.Body);
    }

    private static void InitializerDeclaration(IInitializerDeclarationNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        ConstructorOrInitializerParameters(node.Parameters);
        BlockBody(node.Body);
    }

    private static void FieldDeclaration(IFieldDeclarationNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        Type(node.TypeNode);
        UntypedExpression(node.Initializer);
    }

    private static void AssociatedFunctionDeclaration(IAssociatedFunctionDeclarationNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        NamedParameters(node.Parameters);
        Type(node.Return);
        Body(node.Body);
    }
    #endregion

    #region Attributes
    private static void Attributes(IEnumerable<IAttributeNode> nodes)
        => nodes.ForEach(Attribute);

    private static void Attribute(IAttributeNode node) => TypeName(node.TypeName);
    #endregion

    #region Capabilities
    private static void Capability(ICapabilityNode? node) { }
    #endregion

    #region Parameters
    private static void NamedParameters(IEnumerable<INamedParameterNode> nodes) => nodes.ForEach(NamedParameter);

    private static void NamedParameter(INamedParameterNode node) => Type(node.TypeNode);

    private static void ConstructorOrInitializerParameters(IEnumerable<IConstructorOrInitializerParameterNode> nodes)
        => nodes.ForEach(ConstructorOrInitializerParameter);

    private static void ConstructorOrInitializerParameter(IConstructorOrInitializerParameterNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case INamedParameterNode n:
                NamedParameter(n);
                break;
            case IFieldParameterNode n:
                FieldParameter(n);
                break;
        }
    }

    private static void FieldParameter(IFieldParameterNode node)
        => node.Syntax.ReferencedSymbol.Fulfill(node.ReferencedSymbolNode?.Symbol);
    #endregion

    #region Function Parts
    private static void Body(IBodyNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IBlockBodyNode n:
                BlockBody(n);
                break;
            case IExpressionBodyNode n:
                ExpressionBody(n);
                break;
        }
    }

    private static void BlockBody(IBlockBodyNode node) => BodyStatements(node.Statements);

    private static void ExpressionBody(IExpressionBodyNode node)
        => ResultStatement(node.ResultStatement);
    #endregion

    #region Types
    private static void StandardTypeNames(IEnumerable<IStandardTypeNameNode> nodes)
        => nodes.ForEach(StandardTypeName);

    private static void Types(IFixedList<ITypeNode> nodes)
        => nodes.ForEach(Type);

    private static void Type(ITypeNode? node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case null:
                break;
            case ITypeNameNode n:
                TypeName(n);
                break;
            case IOptionalTypeNode n:
                OptionalType(n);
                break;
            case ICapabilityTypeNode n:
                CapabilityType(n);
                break;
            case IFunctionTypeNode n:
                FunctionType(n);
                break;
            case IViewpointTypeNode n:
                ViewpointType(n);
                break;
        }
    }

    private static void TypeName(ITypeNameNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IStandardTypeNameNode n:
                StandardTypeName(n);
                break;
            case ISimpleTypeNameNode n:
                SimpleTypeName(n);
                break;
            case IQualifiedTypeNameNode _:
                throw new NotImplementedException();
        }
    }

    private static void StandardTypeName(IStandardTypeNameNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IIdentifierTypeNameNode n:
                IdentifierTypeName(n);
                break;
            case IGenericTypeNameNode n:
                GenericTypeName(n);
                break;
        }
    }

    private static void SimpleTypeName(ISimpleTypeNameNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IIdentifierTypeNameNode n:
                IdentifierTypeName(n);
                break;
            case ISpecialTypeNameNode n:
                SpecialTypeName(n);
                break;
        }
    }

    private static void IdentifierTypeName(IIdentifierTypeNameNode node)
    {
        node.Syntax.ReferencedSymbol.Fulfill(node.ReferencedSymbol);
        node.Syntax.NamedType = node.Type;
    }

    private static void GenericTypeName(IGenericTypeNameNode node)
    {
        node.Syntax.ReferencedSymbol.Fulfill(node.ReferencedSymbol);
        node.Syntax.NamedType = node.Type;
        Types(node.TypeArguments);
    }

    private static void SpecialTypeName(ISpecialTypeNameNode node)
    {
        node.Syntax.ReferencedSymbol.Fulfill(node.ReferencedSymbol);
        node.Syntax.NamedType = node.Type;
    }

    private static void OptionalType(IOptionalTypeNode node)
    {
        node.Syntax.NamedType = node.Type;
        Type(node.Referent);
    }

    private static void CapabilityType(ICapabilityTypeNode node)
    {
        node.Syntax.NamedType = node.Type;
        Type(node.Referent);
    }

    private static void FunctionType(IFunctionTypeNode node)
    {
        node.Syntax.NamedType = node.Type;
        ParameterTypes(node.Parameters);
        Type(node.Return);
    }

    private static void ParameterTypes(IEnumerable<IParameterTypeNode> nodes)
        => nodes.ForEach(ParameterType);

    private static void ParameterType(IParameterTypeNode node)
        => Type(node.Referent);

    private static void ViewpointType(IViewpointTypeNode node)
    {
        node.Syntax.NamedType = node.Type;
        Type(node.Referent);
    }
    #endregion

    #region Statements

    private static void Statements(IEnumerable<IStatementNode> node)
        => node.ForEach(Statement);

    private static void Statement(IStatementNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IResultStatementNode n:
                ResultStatement(n);
                break;
            case IBodyStatementNode n:
                BodyStatement(n);
                break;
        }
    }

    private static void ResultStatement(IResultStatementNode node)
        => UntypedExpression(node.Expression);

    private static void BodyStatements(IEnumerable<IBodyStatementNode> nodes)
        => nodes.ForEach(BodyStatement);

    private static void BodyStatement(IBodyStatementNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IVariableDeclarationStatementNode n:
                VariableDeclarationStatement(n);
                break;
            case IExpressionStatementNode n:
                ExpressionStatement(n);
                break;
        }
    }

    private static void VariableDeclarationStatement(IVariableDeclarationStatementNode node)
    {
        Type(node.Type);
        Capability(node.Capability);
        UntypedExpression(node.Initializer);
    }

    private static void ExpressionStatement(IExpressionStatementNode node)
        => UntypedExpression(node.Expression);
    #endregion

    #region Patterns
    private static void Pattern(IPatternNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IBindingContextPatternNode n:
                BindingContextPattern(n);
                break;
            case IOptionalOrBindingPatternNode n:
                OptionalOrBindingPattern(n);
                break;
        }
    }

    private static void BindingContextPattern(IBindingContextPatternNode node)
    {
        Type(node.Type);
        Pattern(node.Pattern);
    }

    private static void OptionalOrBindingPattern(IOptionalOrBindingPatternNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IBindingPatternNode n:
                BindingPattern(n);
                break;
            case IOptionalPatternNode n:
                OptionalPattern(n);
                break;
        }
    }

    private static void BindingPattern(IBindingPatternNode node) { }

    private static void OptionalPattern(IOptionalPatternNode node)
        => OptionalOrBindingPattern(node.Pattern);
    #endregion

    #region Expressions
    private static void UntypedExpressions(IEnumerable<IUntypedExpressionNode> nodes)
        => nodes.ForEach(UntypedExpression);

    private static void UntypedExpression(IUntypedExpressionNode? node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case null:
                break;
            case IExpressionNode n:
                Expression(n);
                break;
            case INameExpressionNode n:
                NameExpression(n);
                break;
        }
    }

    private static void Expression(IExpressionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IAssignableExpressionNode n:
                AssignableExpression(n);
                break;
            case IBlockExpressionNode n:
                BlockExpression(n);
                break;
            case INewObjectExpressionNode n:
                NewObjectExpression(n);
                break;
            case IUnsafeExpressionNode n:
                UnsafeExpression(n);
                break;
            case INeverTypedExpressionNode n:
                NeverTypedExpression(n);
                break;
            case ILiteralExpressionNode n:
                LiteralExpression(n);
                break;
            case IAssignmentExpressionNode n:
                AssignmentExpression(n);
                break;
            case IBinaryOperatorExpressionNode n:
                BinaryOperatorExpression(n);
                break;
            case IUnaryOperatorExpressionNode n:
                UnaryOperatorExpression(n);
                break;
            case IIdExpressionNode n:
                IdExpression(n);
                break;
            case IConversionExpressionNode n:
                ConversionExpression(n);
                break;
            case IPatternMatchExpressionNode n:
                PatternMatchExpression(n);
                break;
            case IIfExpressionNode n:
                IfExpression(n);
                break;
            case ILoopExpressionNode n:
                LoopExpression(n);
                break;
            case IWhileExpressionNode n:
                WhileExpression(n);
                break;
            case IForeachExpressionNode n:
                ForeachExpression(n);
                break;
            case IInvocationExpressionNode n:
                InvocationExpression(n);
                break;
            case ISelfExpressionNode n:
                SelfExpression(n);
                break;
            case IMoveExpressionNode n:
                MoveExpression(n);
                break;
            case IFreezeExpressionNode n:
                FreezeExpression(n);
                break;
            case IAsyncBlockExpressionNode n:
                AsyncBlockExpression(n);
                break;
            case IAsyncStartExpressionNode n:
                AsyncStartExpression(n);
                break;
            case IAwaitExpressionNode n:
                AwaitExpression(n);
                break;
        }
    }

    private static void AssignableExpression(IAssignableExpressionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IIdentifierNameExpressionNode n:
                IdentifierNameExpression(n);
                break;
            case IMemberAccessExpressionNode n:
                MemberAccessExpression(n);
                break;
        }
    }

    private static void BlockExpression(IBlockExpressionNode node) => Statements(node.Statements);

    private static void NewObjectExpression(INewObjectExpressionNode node)
    {
        TypeName(node.Type);
        UntypedExpressions(node.Arguments);
    }

    private static void UnsafeExpression(IUnsafeExpressionNode node)
        => UntypedExpression(node.Expression);

    private static void NeverTypedExpression(INeverTypedExpressionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IBreakExpressionNode n:
                BreakExpression(n);
                break;
            case INextExpressionNode n:
                NextExpression(n);
                break;
            case IReturnExpressionNode n:
                ReturnExpression(n);
                break;
        }
    }

    private static void BreakExpression(IBreakExpressionNode node) { }

    private static void NextExpression(INextExpressionNode node) { }

    private static void ReturnExpression(IReturnExpressionNode node)
        => UntypedExpression(node.Value);
    #endregion

    #region Literal Expressions
    private static void LiteralExpression(ILiteralExpressionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IBoolLiteralExpressionNode n:
                BoolLiteralExpression(n);
                break;
            case IIntegerLiteralExpressionNode n:
                IntegerLiteralExpression(n);
                break;
            case INoneLiteralExpressionNode n:
                NoneLiteralExpression(n);
                break;
            case IStringLiteralExpressionNode n:
                StringLiteralExpression(n);
                break;
        }
    }

    private static void BoolLiteralExpression(IBoolLiteralExpressionNode node) { }

    private static void IntegerLiteralExpression(IIntegerLiteralExpressionNode node) { }

    private static void NoneLiteralExpression(INoneLiteralExpressionNode node) { }

    private static void StringLiteralExpression(IStringLiteralExpressionNode node) { }
    #endregion

    #region Operator Expressions
    private static void AssignmentExpression(IAssignmentExpressionNode node)
    {
        AssignableExpression(node.LeftOperand);
        UntypedExpression(node.RightOperand);
    }

    private static void BinaryOperatorExpression(IBinaryOperatorExpressionNode node)
    {
        UntypedExpression(node.LeftOperand);
        UntypedExpression(node.RightOperand);
    }

    private static void UnaryOperatorExpression(IUnaryOperatorExpressionNode node)
        => UntypedExpression(node.Operand);

    private static void IdExpression(IIdExpressionNode node) => UntypedExpression(node.Referent);

    private static void ConversionExpression(IConversionExpressionNode node)
    {
        UntypedExpression(node.Referent);
        Type(node.ConvertToType);
    }

    private static void PatternMatchExpression(IPatternMatchExpressionNode node)
    {
        UntypedExpression(node.Referent);
        Pattern(node.Pattern);
    }
    #endregion

    #region Invocation Expressions
    private static void InvocationExpression(IInvocationExpressionNode node)
    {
        UntypedExpression(node.Expression);
        UntypedExpressions(node.Arguments);
    }
    #endregion

    #region Control Flow Expressions
    private static void IfExpression(IIfExpressionNode node)
    {
        UntypedExpression(node.Condition);
        BlockOrResult(node.ThenBlock);
        ElseClause(node.ElseClause);
    }

    private static void LoopExpression(ILoopExpressionNode node) => BlockExpression(node.Block);

    private static void WhileExpression(IWhileExpressionNode node)
    {
        UntypedExpression(node.Condition);
        BlockExpression(node.Block);
    }

    private static void ForeachExpression(IForeachExpressionNode node)
    {
        Type(node.Type);
        UntypedExpression(node.InExpression);
        BlockExpression(node.Block);
    }
    #endregion

    #region Name Expressions

    private static void NameExpression(INameExpressionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IIdentifierNameExpressionNode n:
                IdentifierNameExpression(n);
                break;
            case ISpecialTypeNameExpressionNode n:
                SpecialTypeNameExpression(n);
                break;
            case IGenericNameExpressionNode n:
                GenericNameExpression(n);
                break;
            case IMemberAccessExpressionNode n:
                MemberAccessExpression(n);
                break;
            case ISelfExpressionNode n:
                SelfExpression(n);
                break;
        }
    }

    private static void VariableNameExpression(IVariableNameExpressionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IIdentifierNameExpressionNode n:
                IdentifierNameExpression(n);
                break;
            case ISelfExpressionNode n:
                SelfExpression(n);
                break;
        }
    }

    private static void IdentifierNameExpression(IIdentifierNameExpressionNode node) { }

    private static void SpecialTypeNameExpression(ISpecialTypeNameExpressionNode node) { }

    private static void GenericNameExpression(IGenericNameExpressionNode node)
        => Types(node.TypeArguments);

    private static void MemberAccessExpression(IMemberAccessExpressionNode node)
    {
        UntypedExpression(node.Context);
        Types(node.TypeArguments);
    }

    private static void SelfExpression(ISelfExpressionNode node) { }
    #endregion

    #region Capability Expressions
    private static void MoveExpression(IMoveExpressionNode node)
        => VariableNameExpression(node.Referent);

    private static void FreezeExpression(IFreezeExpressionNode node)
        => VariableNameExpression(node.Referent);
    #endregion

    #region Async Expressions
    private static void AsyncBlockExpression(IAsyncBlockExpressionNode node)
        => BlockExpression(node.Block);

    private static void AsyncStartExpression(IAsyncStartExpressionNode node)
        => UntypedExpression(node.Expression);

    private static void AwaitExpression(IAwaitExpressionNode node)
        => UntypedExpression(node.Expression);
    #endregion
}
