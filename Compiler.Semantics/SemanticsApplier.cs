using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
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
    {
        node.CompilationUnits.ForEach(CompilationUnit);
        NamespaceDefinition(node.GlobalNamespace);
    }

    #endregion

    #region Code Files
    private static void CompilationUnit(ICompilationUnitNode node)
        => Definitions(node.Definitions);
    #endregion

    #region Definitions
    private static void Definitions(IEnumerable<IDefinitionNode> nodes)
        => nodes.ForEach(Definition);

    private static void Definition(IDefinitionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case ITypeDefinitionNode n:
                TypeDefinition(n);
                break;
            case IFunctionDefinitionNode n:
                FunctionDefinition(n);
                break;
            case INamespaceBlockDefinitionNode n:
                NamespaceBlockDefinition(n);
                break;
            case IMethodDefinitionNode n:
                MethodDefinition(n);
                break;
            case IConstructorDefinitionNode n:
                ConstructorDefinition(n);
                break;
            case IInitializerDefinitionNode n:
                InitializerDefinition(n);
                break;
            case IFieldDefinitionNode n:
                FieldDefinition(n);
                break;
            case IAssociatedFunctionDefinitionNode n:
                AssociatedFunctionDefinition(n);
                break;
        }
    }
    #endregion

    #region Namespace Definitions
    private static void NamespaceBlockDefinition(INamespaceBlockDefinitionNode node)
    {
        node.Syntax.Symbol.Fulfill(node.Symbol);
        Definitions(node.Members);
    }

    private static void NamespaceDefinitions(IEnumerable<INamespaceDefinitionNode> nodes)
        => nodes.ForEach(NamespaceDefinition);

    private static void NamespaceDefinition(INamespaceDefinitionNode node)
        => NamespaceDefinitions(node.MemberNamespaces);
    #endregion

    #region Function Definition
    private static void FunctionDefinition(IFunctionDefinitionNode node)
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

    #region Type Definitions
    private static void TypeDefinition(ITypeDefinitionNode node)
    {
        var syntax = node.Syntax;
        syntax.Symbol.BeginFulfilling();
        syntax.Symbol.Fulfill(node.Symbol);
        GenericParameters(node.GenericParameters);
        StandardTypeNames(node.AllSupertypeNames);
        Definitions(node.Members);
    }
    #endregion

    #region Type Definition Parts
    private static void GenericParameters(IFixedList<IGenericParameterNode> nodes)
        => nodes.ForEach(GenericParameter);

    private static void GenericParameter(IGenericParameterNode node)
        => node.Syntax.Symbol.Fulfill(node.Symbol);
    #endregion

    #region Member Definitions
    private static void MethodDefinition(IMethodDefinitionNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        SelfParameter(node.SelfParameter);
        NamedParameters(node.Parameters);
        Type(node.Return);

        if (node is IConcreteMethodDefinitionNode n)
            ConcreteMethodDefinition(n);
    }

    private static void ConcreteMethodDefinition(IConcreteMethodDefinitionNode node)
        => Body(node.Body);

    private static void ConstructorDefinition(IConstructorDefinitionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case ISourceConstructorDefinitionNode n:
                SourceConstructorDefinition(n);
                break;
            case IDefaultConstructorDefinitionNode n:
                DefaultConstructorDefinition(n);
                break;
        }
    }

    private static void SourceConstructorDefinition(ISourceConstructorDefinitionNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        SelfParameter(node.SelfParameter);
        ConstructorOrInitializerParameters(node.Parameters);
        BlockBody(node.Body);
    }

    private static void DefaultConstructorDefinition(IDefaultConstructorDefinitionNode node) { }

    private static void InitializerDefinition(IInitializerDefinitionNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        SelfParameter(node.SelfParameter);
        ConstructorOrInitializerParameters(node.Parameters);
        BlockBody(node.Body);
    }

    private static void FieldDefinition(IFieldDefinitionNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        Type(node.TypeNode);
        AmbiguousExpression(node.Initializer);
    }

    private static void AssociatedFunctionDefinition(IAssociatedFunctionDefinitionNode node)
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

    private static void NamedParameter(INamedParameterNode node)
    {
        node.Syntax.Symbol.Fulfill(node.Symbol);
        Type(node.TypeNode);
    }

    private static void SelfParameter(ISelfParameterNode node)
        => node.Syntax.Symbol.Fulfill(node.Symbol);

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
        => node.Syntax.ReferencedSymbol.Fulfill(node.ReferencedField?.Symbol);
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
        => AmbiguousExpression(node.Expression);

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
        // node.Syntax.Symbol.Fulfill(node.Symbol);
        Type(node.Type);
        Capability(node.Capability);
        AmbiguousExpression(node.Initializer);
    }

    private static void ExpressionStatement(IExpressionStatementNode node)
        => AmbiguousExpression(node.Expression);
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
    private static void AmbiguousExpressions(IEnumerable<IAmbiguousExpressionNode> nodes)
        => nodes.ForEach(AmbiguousExpression);

    private static void AmbiguousExpression(IAmbiguousExpressionNode? node)
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
            case IAmbiguousNameExpressionNode n:
                AmbiguousNameExpression(n);
                break;
            case IInvocationExpressionNode n:
                InvocationExpression(n);
                break;
        }
    }

    private static void Expression(IExpressionNode? node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case null:
                break;
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
            case INameExpressionNode n:
                NameExpression(n);
                break;
            case IFunctionInvocationExpressionNode n:
                FunctionInvocationExpression(n);
                break;
            case IMethodInvocationExpressionNode n:
                MethodInvocationExpression(n);
                break;
            case IGetterInvocationExpressionNode n:
                GetterInvocationExpression(n);
                break;
            case ISetterInvocationExpressionNode n:
                SetterInvocationExpression(n);
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
            case IVariableNameExpressionNode n:
                VariableNameExpression(n);
                break;
            case IMemberAccessExpressionNode n:
                MemberAccessExpression(n);
                break;
            case IFieldAccessExpressionNode n:
                FieldAccessExpression(n);
                break;
            case IPropertyNameNode n:
                PropertyName(n);
                break;
            case IMissingNameExpressionNode n:
                MissingNameExpression(n);
                break;
            case IUnknownIdentifierNameExpressionNode n:
                UnknownIdentifierNameExpression(n);
                break;
            case IUnknownMemberAccessExpressionNode n:
                UnknownMemberAccessExpression(n);
                break;
        }
    }

    private static void BlockExpression(IBlockExpressionNode node) => Statements(node.Statements);

    private static void NewObjectExpression(INewObjectExpressionNode node)
    {
        TypeName(node.ConstructingType);
        AmbiguousExpressions(node.Arguments);
    }

    private static void UnsafeExpression(IUnsafeExpressionNode node)
        => AmbiguousExpression(node.Expression);

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
        => AmbiguousExpression(node.Value);
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

    private static void BoolLiteralExpression(IBoolLiteralExpressionNode node)
        => node.Syntax.DataType.Fulfill(node.Type);

    private static void IntegerLiteralExpression(IIntegerLiteralExpressionNode node)
        => node.Syntax.DataType.Fulfill(node.Type);

    private static void NoneLiteralExpression(INoneLiteralExpressionNode _)
    {
        // DataType starts fulfilled
    }

    private static void StringLiteralExpression(IStringLiteralExpressionNode node)
        => node.Syntax.DataType.Fulfill(node.Type);
    #endregion

    #region Operator Expressions
    private static void AssignmentExpression(IAssignmentExpressionNode node)
    {
        AssignableExpression(node.LeftOperand);
        AmbiguousExpression(node.RightOperand);
    }

    private static void BinaryOperatorExpression(IBinaryOperatorExpressionNode node)
    {
        AmbiguousExpression(node.LeftOperand);
        AmbiguousExpression(node.RightOperand);
    }

    private static void UnaryOperatorExpression(IUnaryOperatorExpressionNode node)
        => AmbiguousExpression(node.Operand);

    private static void IdExpression(IIdExpressionNode node) => AmbiguousExpression(node.Referent);

    private static void ConversionExpression(IConversionExpressionNode node)
    {
        AmbiguousExpression(node.Referent);
        Type(node.ConvertToType);
    }

    private static void PatternMatchExpression(IPatternMatchExpressionNode node)
    {
        AmbiguousExpression(node.Referent);
        Pattern(node.Pattern);
    }
    #endregion

    #region Invocation Expressions
    private static void InvocationExpression(IInvocationExpressionNode node)
    {
        AmbiguousExpression(node.Expression);
        AmbiguousExpressions(node.Arguments);
    }

    private static void FunctionInvocationExpression(IFunctionInvocationExpressionNode node)
    {
        FunctionGroupName(node.FunctionGroup);
        AmbiguousExpressions(node.Arguments);
    }

    private static void MethodInvocationExpression(IMethodInvocationExpressionNode node)
    {
        MethodGroupName(node.MethodGroup);
        AmbiguousExpressions(node.Arguments);
    }

    private static void GetterInvocationExpression(IGetterInvocationExpressionNode node)
        => Expression(node.Context);


    private static void SetterInvocationExpression(ISetterInvocationExpressionNode node)
    {
        Expression(node.Context);
        AmbiguousExpression(node.Value);
    }
    #endregion

    #region Control Flow Expressions
    private static void IfExpression(IIfExpressionNode node)
    {
        AmbiguousExpression(node.Condition);
        BlockOrResult(node.ThenBlock);
        ElseClause(node.ElseClause);
    }

    private static void LoopExpression(ILoopExpressionNode node) => BlockExpression(node.Block);

    private static void WhileExpression(IWhileExpressionNode node)
    {
        AmbiguousExpression(node.Condition);
        BlockExpression(node.Block);
    }

    private static void ForeachExpression(IForeachExpressionNode node)
    {
        Type(node.DeclaredType);
        AmbiguousExpression(node.InExpression);
        BlockExpression(node.Block);
    }
    #endregion

    #region Ambiguous Name Expressions
    private static void AmbiguousNameExpression(IAmbiguousNameExpressionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case INameExpressionNode n:
                NameExpression(n);
                break;
            case IIdentifierNameExpressionNode n:
                IdentifierNameExpression(n);
                break;
            case IGenericNameExpressionNode n:
                GenericNameExpression(n);
                break;
            case IMemberAccessExpressionNode n:
                MemberAccessExpression(n);
                break;
            case IPropertyNameNode n:
                PropertyName(n);
                break;
        }
    }

    private static void SimpleName(ISimpleNameNode node)
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
            case IMissingNameExpressionNode n:
                MissingNameExpression(n);
                break;
            case IVariableNameExpressionNode n:
                VariableNameExpression(n);
                break;
            case IUnknownIdentifierNameExpressionNode n:
                UnknownIdentifierNameExpression(n);
                break;
        }
    }

    private static void IdentifierNameExpression(IIdentifierNameExpressionNode node) { }

    private static void GenericNameExpression(IGenericNameExpressionNode node)
        => Types(node.TypeArguments);

    private static void MemberAccessExpression(IMemberAccessExpressionNode node)
    {
        AmbiguousExpression(node.Context);
        Types(node.TypeArguments);
    }

    private static void PropertyName(IPropertyNameNode node) => Expression(node.Context);
    #endregion

    #region Name Expressions
    private static void NameExpression(INameExpressionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case INamespaceNameNode n:
                NamespaceName(n);
                break;
            case IFunctionGroupNameNode n:
                FunctionGroupName(n);
                break;
            case IMethodGroupNameNode n:
                MethodGroupName(n);
                break;
            case IFieldAccessExpressionNode n:
                FieldAccessExpression(n);
                break;
            case IVariableNameExpressionNode n:
                VariableNameExpression(n);
                break;
            case IStandardTypeNameExpressionNode n:
                StandardTypeNameExpression(n);
                break;
            case IQualifiedTypeNameExpressionNode n:
                QualifiedTypeNameExpression(n);
                break;
            case IInitializerGroupNameNode n:
                InitializeGroupName(n);
                break;
            case ISpecialTypeNameExpressionNode n:
                SpecialTypeNameExpression(n);
                break;
            case ISelfExpressionNode n:
                SelfExpression(n);
                break;
            case IMissingNameExpressionNode n:
                MissingNameExpression(n);
                break;
            case IUnknownNameExpressionNode n:
                UnknownNameExpression(n);
                break;
        }
    }

    private static void NamespaceName(INamespaceNameNode? node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case null:
                break;
            case IUnqualifiedNamespaceNameNode n:
                UnqualifiedNamespaceName(n);
                break;
            case IQualifiedNamespaceNameNode n:
                QualifiedNamespaceName(n);
                break;
        }
    }

    private static void UnqualifiedNamespaceName(IUnqualifiedNamespaceNameNode node)
    {
        var syntax = node.Syntax;
        syntax.Semantics.Fulfill(
            new NamespaceNameSyntax(node.ReferencedDeclarations.Select(d => d.Symbol)
                                        .Cast<LocalNamespaceSymbol>().ToFixedSet()));
    }

    private static void QualifiedNamespaceName(IQualifiedNamespaceNameNode node)
    {
        var namespaceSymbols = node.ReferencedDeclarations.Select(d => d.Symbol).Cast<LocalNamespaceSymbol>().ToFixedSet();
        node.Syntax.Semantics.Fulfill(new NamespaceNameSyntax(namespaceSymbols));
        NamespaceName(node.Context);
    }

    private static void FunctionGroupName(IFunctionGroupNameNode node)
    {
        var semantics = new FunctionGroupNameSyntax(
            node.ReferencedDeclarations.Select(d => d.Symbol).ToFixedSet());
        switch (node.Syntax)
        {
            case IIdentifierNameExpressionSyntax syntax:
                syntax.Semantics.Fulfill(semantics);
                break;
            case IMemberAccessExpressionSyntax syntax:
                syntax.Semantics.Fulfill(semantics);
                break;
        }
        Expression(node.Context);
    }

    private static void MethodGroupName(IMethodGroupNameNode node)
    {
        var semantics = new MethodGroupNameSyntax(
                       node.ReferencedDeclarations.Select(d => d.Symbol).ToFixedSet());
        node.Syntax.Semantics.Fulfill(semantics);
        Expression(node.Context);
    }

    private static void FieldAccessExpression(IFieldAccessExpressionNode node)
    {
        var semantics = new FieldNameExpressionSyntax(node.ReferencedDeclaration.Symbol);
        node.Syntax.Semantics.Fulfill(semantics);
        Expression(node.Context);
    }

    private static void VariableNameExpression(IVariableNameExpressionNode node)
    {
        var syntax = node.Syntax;
        syntax.Semantics.Fulfill(new NamedVariableNameSyntax(node.ReferencedDeclaration.Syntax.Symbol));
    }

    private static void StandardTypeNameExpression(IStandardTypeNameExpressionNode node)
    {
        var semantics = new TypeNameSyntax(node.ReferencedDeclaration.Symbol);
        switch (node.Syntax)
        {
            default:
                throw ExhaustiveMatch.Failed(node.Syntax);
            case IIdentifierNameExpressionSyntax syntax:
                syntax.Semantics.Fulfill(semantics);
                break;
            case IGenericNameExpressionSyntax syntax:
                //syntax.Semantics.Fulfill(semantics);
                throw new NotImplementedException("Generic names not implemented in syntax yet");
        }
    }

    private static void QualifiedTypeNameExpression(IQualifiedTypeNameExpressionNode node)
    {
        node.Syntax.Semantics.Fulfill(new TypeNameSyntax(node.ReferencedDeclaration.Symbol));
        Expression(node.Context);
        Types(node.TypeArguments);
    }

    private static void InitializeGroupName(IInitializerGroupNameNode node)
    {
        var symbols = node.ReferencedDeclarations.Select(d => d.Symbol).ToFixedSet();
        node.Syntax.Semantics.Fulfill(new InitializerGroupNameSyntax(symbols));
        Expression(node.Context);
    }

    private static void SpecialTypeNameExpression(ISpecialTypeNameExpressionNode node)
    {
        var syntax = node.Syntax;
        syntax.ReferencedSymbol.Fulfill(node.ReferencedSymbol);
    }

    private static void SelfExpression(ISelfExpressionNode node)
    {
        var semantics = node.ReferencedSymbol is not null
            ? new SelfExpressionSyntax(node.ReferencedSymbol)
            : (ISelfExpressionSyntaxSemantics)UnknownNameSyntax.Instance;
        node.Syntax.Semantics.Fulfill(semantics);
    }

    private static void MissingNameExpression(IMissingNameExpressionNode node) { }

    private static void UnknownNameExpression(IUnknownNameExpressionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IUnknownStandardNameExpressionNode n:
                UnknownStandardNameExpression(n);
                break;
            case IUnknownMemberAccessExpressionNode n:
                UnknownMemberAccessExpression(n);
                break;
        }
    }

    private static void UnknownStandardNameExpression(IUnknownStandardNameExpressionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IUnknownIdentifierNameExpressionNode n:
                UnknownIdentifierNameExpression(n);
                break;

            case IUnknownGenericNameExpressionNode n:
                UnknownGenericNameExpression(n);
                break;
        }
    }

    private static void UnknownIdentifierNameExpression(IUnknownIdentifierNameExpressionNode node)
        => node.Syntax.Semantics.Fulfill(UnknownNameSyntax.Instance);

    private static void UnknownGenericNameExpression(IUnknownGenericNameExpressionNode node)
        => throw new NotImplementedException("Generic names not implement in parser yet");

    private static void UnknownMemberAccessExpression(IUnknownMemberAccessExpressionNode node)
    {
        // Do not apply this in other contexts yet because the semantic tree cannot always properly
        // resolve members
        // TODO apply all the time once the semantic tree is more complete
        if (node.Context is ITypeNameExpressionNode
            or INamespaceNameNode
            or IUnknownMemberAccessExpressionNode)
            node.Syntax.Semantics.Fulfill(UnknownNameSyntax.Instance);
        Expression(node.Context);
    }
    #endregion

    #region Capability Expressions
    private static void MoveExpression(IMoveExpressionNode node)
        => SimpleName(node.Referent);

    private static void FreezeExpression(IFreezeExpressionNode node)
        => SimpleName(node.Referent);
    #endregion

    #region Async Expressions
    private static void AsyncBlockExpression(IAsyncBlockExpressionNode node)
        => BlockExpression(node.Block);

    private static void AsyncStartExpression(IAsyncStartExpressionNode node)
        => AmbiguousExpression(node.Expression);

    private static void AwaitExpression(IAwaitExpressionNode node)
        => AmbiguousExpression(node.Expression);
    #endregion
}
