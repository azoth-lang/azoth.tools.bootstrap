namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

internal class SemanticTreeValidator
{
    public static void Validate(ISemanticNode node)
    {
        if (node is IDeclarationNode d)
        {
            _ = d.ContainingLexicalScope;
        }

        switch (node)
        {
            case IPackageNode n:
                _ = n.SymbolNodes;
                _ = n.PackageNameScope;
                break;
            case IPackageFacetNode n:
                _ = n.Declarations;
                _ = n.LexicalScope;
                break;
            case ICompilationUnitNode n:
                _ = n.ContainingLexicalScope;
                _ = n.LexicalScope;
                break;
            case INamespaceDeclarationNode n:
                _ = n.Symbol;
                break;
            case IClassDeclarationNode n:
                _ = n.File;
                break;
            case IStructDeclarationNode n:
                _ = n.File;
                break;
            case ITraitDeclarationNode n:
                _ = n.File;
                break;
            case IFunctionDeclarationNode n:
                _ = n.File;
                break;
        }

        foreach (var child in node.Children())
            Validate(child);
    }
}
