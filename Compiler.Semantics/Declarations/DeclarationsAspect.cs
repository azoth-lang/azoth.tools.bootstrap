using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
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
    // TODO move to TypeConstructorsAspect and find an implementation without ContainingDeclaration?
    public static partial ITypeConstructor GenericParameterDeclaration_TypeConstructor(IGenericParameterDeclarationNode node)
        => node.ContainingDeclaration.TypeConstructor.ParameterTypeFactories.Single(p => p.Parameter.Name == node.Name);
    #endregion
}
