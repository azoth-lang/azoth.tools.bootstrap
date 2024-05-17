using System;

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
            _ = typeDecl.DeclaredType;
            _ = typeDecl.Supertypes;
        }
        if (node is ITypeNameNode typeName)
        {
            _ = typeName.ContainingLexicalScope;
            _ = typeName.ReferencedSymbol;
            _ = typeName.BareType;
            _ = typeName.Type;
        }
        if (node is IStandardTypeNameNode standardTypeName)
        {
            _ = standardTypeName.ReferencedSymbolNode;
        }
        if (node is ITypeNode type)
        {
            _ = type.Type;
        }

        // Validate Concrete Node Attributes
        switch (node)
        {
            case IPackageNode n:
                _ = n.PackageDeclarations;
                break;
            case IPackageFacetNode n:
                _ = n.Definitions;
                _ = n.PackageNameScope;
                break;
            case ICompilationUnitNode n:
                _ = n.ContainingLexicalScope;
                _ = n.LexicalScope;
                break;
            case INamespaceBlockDefinitionNode n:
                _ = n.Symbol;
                _ = n.ContainingLexicalScope;
                _ = n.LexicalScope;
                break;
            case IClassDefinitionNode n:
                _ = n.File;
                _ = n.DeclaredType;
                _ = n.Symbol;
                break;
            case IStructDefinitionNode n:
                _ = n.File;
                _ = n.DeclaredType;
                _ = n.Symbol;
                break;
            case ITraitDefinitionNode n:
                _ = n.File;
                _ = n.DeclaredType;
                _ = n.Symbol;
                break;
            case IGenericParameterNode n:
                _ = n.Parameter;
                _ = n.ContainingDeclaredType;
                _ = n.DeclaredType;
                _ = n.Symbol;
                break;
            case IFunctionDefinitionNode n:
                _ = n.File;
                _ = n.Type;
                _ = n.Symbol;
                break;
            case IAttributeNode n:
                _ = n.TypeName.ReferencedSymbol;
                break;
            case IIdentifierNameExpressionNode n:
                if (n.Name is null)
                    throw new InvalidOperationException("Nodes with null name ought to be rewritten away");
                break;
            case IMissingNameExpressionNode _:
                break;
        }

        foreach (var child in node.Children())
            Validate(child);
    }
}
