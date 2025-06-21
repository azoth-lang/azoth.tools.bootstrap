using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

/// <summary>
/// Defines lexical scopes. The <c>LexicalScope()</c> attribute gives the lexical scope defined by
/// or in effect on a node. The <c>ContainingLexicalScope()</c> attribute gives the lexical scope
/// that a node is inside of. The <c>FlowLexicalScope()</c> attribute provides a means of managing
/// lexical scopes as they flow through expressions. For example, the condition of an <c>if</c>
/// might contain a pattern match that declares a new binding. It is a <see
/// cref="ConditionalLexicalScope"/> because what variables are in scope can depend on the truth
/// value of the expression. The <c>FlowLexicalScope()</c> of a node gives the lexical scope(s) in
/// effect after the expression is evaluated.
/// </summary>
internal static partial class LexicalScopingAspect
{
    #region Special Parts
    public static partial LexicalScope BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(IBodyOrBlockNode node, int statementIndex)
    {
        if (statementIndex == 0)
            return node.ContainingLexicalScope();
        return node.Statements[statementIndex - 1].LexicalScope();
    }
    #endregion

    #region Packages
    public static partial PackageNameScope Package_MainFacet_PackageNameScope(IPackageNode node)
        => new([node.MainFacet],
            node.MainFacet.References.Append(node.MainFacet.IntrinsicsReference).Select(r => r.SymbolNode),
            node.PrimitivesDeclarations);

    public static partial PackageNameScope Package_TestingFacet_PackageNameScope(IPackageNode node)
        => new([node.TestingFacet],
            // TODO append main facet reference
            node.TestingFacet.References.Append(node.TestingFacet.IntrinsicsReference).Select(r => r.SymbolNode),
            node.PrimitivesDeclarations);
    #endregion

    #region Code Files
    public static partial NamespaceSearchScope CompilationUnit_LexicalScope(ICompilationUnitNode node)
        => BuildNamespaceScope(node.ContainingLexicalScope, node.ImplicitNamespaceName, node.ImportDirectives);

    private static NamespaceSearchScope BuildNamespaceScope(
        NamespaceSearchScope containingLexicalScope,
        NamespaceName namespaceName,
        IFixedList<IImportDirectiveNode> importDirectives)
    {
        var namespaceScope = GetNamespaceScope(containingLexicalScope, namespaceName);
        var lexicalScope = BuildImportDirectivesScope(namespaceScope, importDirectives);
        return lexicalScope;
    }

    private static NamespaceScope GetNamespaceScope(
        NamespaceSearchScope containingLexicalScope, NamespaceName namespaceName)
    {
        var lexicalScope = containingLexicalScope;
        foreach (var ns in namespaceName.Segments)
            lexicalScope = lexicalScope.GetChildNamespaceScope(ns)
                           ?? throw new UnreachableException("Should always be getting namespace names that correspond to definitions.");
        // Either CreateChildNamespaceScope was called, or this is a compilation unit and the
        // original containingLexicalScope was a NamespaceScope.
        return (NamespaceScope)lexicalScope;
    }

    private static NamespaceSearchScope BuildImportDirectivesScope(
        NamespaceScope containingScope,
        IFixedList<IImportDirectiveNode> importDirectives)
    {
        if (importDirectives.IsEmpty) return containingScope;

        var globalScope = containingScope.PackageNames.ImportGlobalScope;
        // TODO put a NamespaceScope attribute on the using directive node for this
        // TODO report an error if the import directive refers to a namespace that doesn't exist
        var namespaceScopes = importDirectives.Select(d => GetNamespaceScope(globalScope, d.Name));

        return new ImportDirectivesScope(containingScope, namespaceScopes);
    }
    #endregion

    #region Namespace Definitions
    public static partial NamespaceSearchScope NamespaceBlockDefinition_LexicalScope(INamespaceBlockDefinitionNode node)
    {
        var containingLexicalScope = node.ContainingLexicalScope;
        if (node.IsGlobalQualified)
            containingLexicalScope = containingLexicalScope.PackageNames.PackageGlobalScope;
        return BuildNamespaceScope(containingLexicalScope, node.DeclaredNames, node.ImportDirectives);
    }
    #endregion

    #region Type Definitions
    public static partial LexicalScope TypeDefinition_SupertypesLexicalScope(ITypeDefinitionNode node)
        // The `Self` type is always available even if there are no generic parameters
        => new DeclarationScope(node.ContainingLexicalScope,
            node.GenericParameters.Append<INamedDeclarationNode>(node.ImplicitSelf));

    public static partial LexicalScope TypeDefinition_LexicalScope(ITypeDefinitionNode node)
        // Only associated (i.e. "static") names are in scope. Other names must use `self.`
        => new DeclarationScope(node.SupertypesLexicalScope,
            node.Members.OfType<IAssociatedMemberDefinitionNode>());

    public static partial LexicalScope TypeDefinition_AllSupertypeNames_Broadcast_ContainingLexicalScope(ITypeDefinitionNode node)
        => node.SupertypesLexicalScope;

    public static partial LexicalScope TypeDefinition_Members_Broadcast_ContainingLexicalScope(ITypeDefinitionNode node)
        => node.LexicalScope;
    #endregion

    #region Function Definition
    public static partial LexicalScope FunctionDefinition_LexicalScope(IFunctionDefinitionNode node)
        // TODO create a type parameter scope when type parameters are supported
        => new DeclarationScope(node.ContainingLexicalScope, node.Parameters);
    #endregion

    #region Member Definitions
    public static partial LexicalScope MethodDefinition_LexicalScope(IMethodDefinitionNode node)
        // TODO create a type parameter scope when type parameters are supported
        => new DeclarationScope(node.ContainingLexicalScope, node.Parameters);

    public static partial LexicalScope InitializerDefinition_LexicalScope(IInitializerDefinitionNode node)
        => new DeclarationScope(node.ContainingLexicalScope, node.Parameters.OfType<INamedDeclarationNode>());

    public static partial LexicalScope AssociatedFunctionDefinition_LexicalScope(IAssociatedFunctionDefinitionNode node)
        // TODO create a type parameter scope when type parameters are supported
        => new DeclarationScope(node.ContainingLexicalScope, node.Parameters);
    #endregion

    #region Statements
    public static partial LexicalScope VariableDeclarationStatement_LexicalScope(IVariableDeclarationStatementNode node)
        => new DeclarationScope(node.ContainingLexicalScope, node);
    #endregion

    #region Patterns
    public static partial ConditionalLexicalScope TypePattern_FlowLexicalScope(ITypePatternNode node)
        => ConditionalLexicalScope.Unconditional(node.ContainingLexicalScope);

    public static partial ConditionalLexicalScope BindingPattern_FlowLexicalScope(IBindingPatternNode node)
    {
        var containingLexicalScope = node.ContainingLexicalScope;
        var variableScope = new DeclarationScope(containingLexicalScope, node);
        return new(variableScope, containingLexicalScope);
    }
    #endregion

    #region Expressions
    /// <summary>
    /// Default implementation for expressions that can't introduce a new scope.
    /// </summary>
    public static partial ConditionalLexicalScope AmbiguousExpression_FlowLexicalScope(IAmbiguousExpressionNode node)
       => ConditionalLexicalScope.Unconditional(node.ContainingLexicalScope());
    #endregion

    #region Operator Expressions
    /// <remarks>In an assignment, the left hand side cannot use variables declared in the right-hand
    /// side because parts of it will be evaluated first. For simplicity, both sides are evaluated
    /// in the containing lexical scope and only the right hand scope is used.</remarks>
    public static partial ConditionalLexicalScope AssignmentExpression_FlowLexicalScope(IAssignmentExpressionNode node)
        => node.TempRightOperand.FlowLexicalScope();

    public static partial ConditionalLexicalScope BinaryOperatorExpression_FlowLexicalScope(IBinaryOperatorExpressionNode node)
    {
        if (node.Operator == BinaryOperator.Or)
            // Cannot statically be sure which part of the expression was evaluated
            return ConditionalLexicalScope.Unconditional(node.ContainingLexicalScope());
        return node.TempRightOperand.FlowLexicalScope();
    }

    public static partial LexicalScope BinaryOperatorExpression_RightOperand_Broadcast_ContainingLexicalScope(IBinaryOperatorExpressionNode node)
    {
        var flowScope = node.TempLeftOperand.FlowLexicalScope();
        // Logical-or is short-circuiting, so the right operand is only evaluated if the left
        // operand is false. Otherwise, the operator is logical-and or another operator and the
        // right operand will be evaluated when the left operand is true (or always).
        return node.Operator == BinaryOperator.Or ? flowScope.False : flowScope.True;
    }

    public static partial ConditionalLexicalScope UnaryOperatorExpression_FlowLexicalScope(IUnaryOperatorExpressionNode node)
    {
        var flow = node.TempOperand.FlowLexicalScope();
        return node.Operator == UnaryOperator.Not ? flow.Swapped() : flow;
    }
    #endregion

    #region Control Flow Expressions
    public static partial LexicalScope ForeachExpression_LexicalScope(IForeachExpressionNode node)
    {
        var flowScope = node.TempInExpression.FlowLexicalScope().True;
        return new DeclarationScope(flowScope, node);
    }
    #endregion
}
