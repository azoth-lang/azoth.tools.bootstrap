namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

internal class SemanticTreeValidator
{
    public static void Validate(ISemanticNode node)
    {
        // Validate Abstract Node Attributes
        if (node is IDefinitionNode decl)
        {
            _ = decl.ContainingLexicalScope;
        }

        if (node is ITypeDefinitionNode typeDecl)
        {
            _ = typeDecl.TypeConstructor;
            _ = typeDecl.Supertypes;
        }
        if (node is ITypeNameNode typeName)
        {
            _ = typeName.ContainingLexicalScope;
            _ = typeName.NamedBareType;
            _ = typeName.NamedType;
        }
        if (node is IOrdinaryTypeNameNode standardTypeName)
        {
            _ = standardTypeName.ReferencedDeclaration;
        }
        if (node is ITypeNode type)
        {
            _ = type.NamedType;
        }
        if (node is IParameterNode parameter)
        {
            _ = parameter.BindingValueId;
        }
        if (node is IExpressionNode expression)
        {
            _ = expression.ValueId;
        }
        if (node is IOrdinaryTypedExpressionNode ordinaryTypedExpression)
        {
            ordinaryTypedExpression.ImplicitRecoveryAllowed();
            ordinaryTypedExpression.ShouldPrepareToReturn();
        }
        if (node is IExecutableDefinitionNode executable)
        {
            _ = executable.VariableBindingsMap;
            // NTA nodes are not a children but should still be validated
            Validate(executable.Entry);
            Validate(executable.Exit);
        }
        if (node is IControlFlowNode controlFlow)
        {
            _ = controlFlow.ControlFlowNext;
            _ = controlFlow.ControlFlowPrevious;
        }
        if (node is IDataFlowNode dataFlow)
        {
            _ = dataFlow.DataFlowPrevious;
            _ = dataFlow.DefinitelyAssigned;
            _ = dataFlow.DefinitelyUnassigned;
        }

        // Validate Concrete Node Attributes
        switch (node)
        {
            case IPackageFacetNode n:
                _ = n.Definitions;
                _ = n.Diagnostics;
                _ = n.Symbols;
                _ = n.PackageNameScope;
                break;
            case ICompilationUnitNode n:
                _ = n.ContainingLexicalScope;
                _ = n.LexicalScope;
                _ = n.Diagnostics;
                break;
            case INamespaceBlockDefinitionNode n:
                _ = n.Symbol;
                _ = n.ContainingLexicalScope;
                _ = n.LexicalScope;
                break;
            case IClassDefinitionNode n:
                _ = n.File;
                _ = n.TypeConstructor;
                _ = n.Symbol;
                break;
            case IStructDefinitionNode n:
                _ = n.File;
                _ = n.TypeConstructor;
                _ = n.Symbol;
                break;
            case ITraitDefinitionNode n:
                _ = n.File;
                _ = n.TypeConstructor;
                _ = n.Symbol;
                break;
            case IGenericParameterNode n:
                _ = n.Parameter;
                _ = n.ContainingTypeConstructor;
                _ = n.DeclaredType;
                _ = n.Symbol;
                break;
            case IFunctionDefinitionNode n:
                _ = n.File;
                _ = n.Type;
                _ = n.Symbol;
                break;
            case IMissingNameExpressionNode _:
                break;
            case ISelfExpressionNode n:
                _ = n.ContainingDeclaration;
                break;
            case IVariableDeclarationStatementNode n:
                _ = n.BindingValueId;
                break;
            case IReturnExpressionNode n:
                // Force flow state to the return where it matters
                _ = n.Value?.FlowStateAfter;
                break;
        }

        foreach (var child in node.Children())
            Validate(child);
    }
}
