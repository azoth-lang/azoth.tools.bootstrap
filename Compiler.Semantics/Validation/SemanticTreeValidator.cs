namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

internal class SemanticTreeValidator
{
    public static void Validate(ISemanticNode node)
    {
        switch (node)
        {
            case IPackageNode n:
                _ = n.SymbolNodes;
                break;
            case IPackageFacetNode n:
                _ = n.Declarations;
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
