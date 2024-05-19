using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

internal static class BindingAmbiguousNamesAspect
{
    public static IFixedList<IDeclarationNode> StandardNameExpression_ReferencedDeclarations(IStandardNameExpressionNode node)
        => node.ContainingLexicalScope.Lookup(node.Name).ToFixedList();

    public static IUnqualifiedNamespaceNameNode? IdentifierName_Rewrite(IIdentifierNameExpressionNode node)
    {
        var referencedNamespaces = node.ReferencedDeclarations.OfType<INamespaceDeclarationNode>().ToFixedList();
        // If not all referenced declarations are namespaces, then this is not a namespace name.
        if (referencedNamespaces.Count != node.ReferencedDeclarations.Count)
            return null;

        return new UnqualifiedNamespaceNameNode(node.Syntax, referencedNamespaces);
    }
}
