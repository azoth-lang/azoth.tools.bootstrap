using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Declarations;

/// <summary>
/// Defines the declaration node types.
/// </summary>
internal static partial class DeclarationsAspect
{
    #region Namespace Declarations
    public static partial IFixedList<INamespaceMemberDeclarationNode> NamespaceDeclaration_NestedMembers(INamespaceDeclarationNode node)
        => node.Members.OfType<INamespaceDeclarationNode>()
               .SelectMany(ns => ns.Members.Concat(ns.NestedMembers)).ToFixedList();
    #endregion

    #region Type Declarations
    public static partial ITypeConstructor GenericParameterDeclaration_TypeFactory(IGenericParameterDeclarationNode node)
        => node.ContainingDeclaration.TypeFactory.ParameterTypeFactories.Single(p => p.Parameter.Name.Equals(node.Name));
    #endregion
}
