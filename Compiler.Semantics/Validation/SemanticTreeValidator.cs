namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

internal class SemanticTreeValidator
{
    public static void Validate(ISemanticNode node)
    {
        // Validate Abstract Node Attributes
        switch (node)
        {
            case IDeclarationNode d:
                _ = d.ContainingLexicalScope;
                break;
            case ITypeNameNode t:
                _ = t.ContainingLexicalScope;
                break;
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
                break;
            case IIdentifierTypeNameNode n:
                _ = n.ReferencedSymbolNode;
                break;
            case IGenericTypeNameNode n:
                _ = n.ReferencedSymbolNode;
                break;
            case ISpecialTypeNameNode n:
                _ = n.ReferencedSymbol;
                break;
        }

        foreach (var child in node.Children())
            Validate(child);
    }
}
