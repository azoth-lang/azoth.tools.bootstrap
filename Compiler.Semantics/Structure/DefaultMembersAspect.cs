using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class DefaultMembersAspect
{
    #region Type Definitions
    public static partial IImplicitSelfDefinitionNode TypeDefinition_ImplicitSelf(ITypeDefinitionNode node)
        => IImplicitSelfDefinitionNode.Create();

    /// <remarks>This needs to be lazy computed because the
    /// <see cref="IClassDefinitionNode.DefaultInitializer"/> attribute must be computed.</remarks>
    public static partial IFixedSet<ITypeMemberDefinitionNode> ClassDefinition_Members(IClassDefinitionNode node)
    {
        var members = node.SourceMembers.AsEnumerable();

        if (node.DefaultInitializer is { } defaultInitializer)
            members = members.Append(defaultInitializer);

        return members.ToFixedSet();
    }

    public static partial IDefaultInitializerDefinitionNode? ClassDefinition_DefaultInitializer(IClassDefinitionNode node)
    {
        if (node.SourceMembers.Any(m => m is IInitializerDefinitionNode)) return null;
        return Child.Attach(node, IDefaultInitializerDefinitionNode.Create());
    }

    /// <remarks>This needs to be lazy computed because the
    /// <see cref="IStructDefinitionNode.DefaultInitializer"/> attribute must be computed.</remarks>
    public static partial IFixedSet<ITypeMemberDefinitionNode> StructDefinition_Members(IStructDefinitionNode node)
    {
        var members = node.SourceMembers.AsEnumerable();

        if (node.DefaultInitializer is { } defaultInitializer)
            members = members.Append(defaultInitializer);

        return members.ToFixedSet();
    }

    public static partial IDefaultInitializerDefinitionNode? StructDefinition_DefaultInitializer(IStructDefinitionNode node)
    {
        if (node.SourceMembers.Any(m => m is IInitializerDefinitionNode)) return null;
        return Child.Attach(node, IDefaultInitializerDefinitionNode.Create());
    }
    #endregion
}
