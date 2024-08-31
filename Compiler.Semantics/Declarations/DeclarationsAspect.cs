using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Declarations;

/// <summary>
/// Defines the declaration node types.
/// </summary>
internal static partial class DeclarationsAspect
{
    public static partial IFixedList<INamespaceMemberDeclarationNode> NamespaceDeclaration_NestedMembers(INamespaceDeclarationNode node)
        => node.Members.OfType<INamespaceDeclarationNode>()
               .SelectMany(ns => ns.Members.Concat(ns.NestedMembers)).ToFixedList();
}
