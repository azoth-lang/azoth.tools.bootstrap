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
            case INamespaceDeclarationNode n:
                _ = n.Symbol;
                break;
        }

        foreach (var child in node.Children())
            Validate(child);
    }
}
