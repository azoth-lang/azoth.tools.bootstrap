using System.Collections.Generic;
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
        if (node.ReferencedDeclarations.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return null;

        return new UnqualifiedNamespaceNameNode(node.Syntax, referencedNamespaces);
    }

    public static IQualifiedNamespaceNameNode? MemberAccessExpression_Rewrite(IMemberAccessExpressionNode node)
    {
        if (node.Context is not INamespaceNameNode context)
            return null;

        var members = context.ReferencedDeclarations.SelectMany(d => d.MembersNamed(node.MemberName)).ToFixedSet();
        if (members.Count == 0)
            return null;

        if (members.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return new QualifiedNamespaceNameNode(node.Syntax, context, referencedNamespaces);

        return null;
    }

    private static bool TryAllOfType<T>(
        this IReadOnlyCollection<IDeclarationNode> declarations,
        out IFixedList<T> referencedNamespaces)
        where T : IDeclarationNode
    {
        referencedNamespaces = declarations.OfType<T>().ToFixedList();
        // If not all referenced declarations are namespaces, then this is not a namespace name.
        if (referencedNamespaces.Count != declarations.Count) return true;
        return false;
    }
}
