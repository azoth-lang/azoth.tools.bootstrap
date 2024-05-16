using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

internal class SemanticTreeValidator
{
    public static void Validate(ISemanticNode node)
    {
        // Validate Abstract Node Attributes
        if (node is IDeclarationNode decl)
        {
            _ = decl.ContainingLexicalScope;
        }

        if (node is ITypeDeclarationNode typeDecl)
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
                _ = n.SymbolNodes;
                break;
            case IPackageFacetNode n:
                _ = n.Declarations;
                _ = n.PackageNameScope;
                break;
            case ICompilationUnitNode n:
                _ = n.ContainingLexicalScope;
                _ = n.LexicalScope;
                break;
            case INamespaceDeclarationNode n:
                _ = n.Symbol;
                _ = n.ContainingLexicalScope;
                _ = n.LexicalScope;
                break;
            case IClassDeclarationNode n:
                _ = n.File;
                _ = n.DeclaredType;
                _ = n.Symbol;
                break;
            case IStructDeclarationNode n:
                _ = n.File;
                _ = n.DeclaredType;
                _ = n.Symbol;
                break;
            case ITraitDeclarationNode n:
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
            case IFunctionDeclarationNode n:
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
