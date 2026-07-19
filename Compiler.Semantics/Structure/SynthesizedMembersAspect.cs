using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class SynthesizedMembersAspect
{
    #region Type Definitions
    public static partial IImplicitSelfDefinitionNode TypeDefinition_ImplicitSelf(ITypeDefinitionNode node)
        => IImplicitSelfDefinitionNode.Create();

    /// <remarks>This needs to be lazy computed because the
    /// <see cref="ITypeDefinitionNode.SynthesizedMembers"/> attribute must be computed.</remarks>
    public static partial IFixedSet<ITypeMemberDefinitionNode> TypeDefinition_DeclaredMembers(ITypeDefinitionNode node)
        => node.SourceMembers.Concat(node.SynthesizedMembers).ToFixedSet();

    public static partial IDefaultInitializerDefinitionNode? ClassDefinition_DefaultInitializer(IClassDefinitionNode node)
    {
        if (node.SourceMembers.Any(m => m is IInitializerDefinitionNode)) return null;
        return Child.Attach(node, IDefaultInitializerDefinitionNode.Create());
    }

    /// <remarks>This needs to be lazy computed because the
    /// <see cref="IClassDefinitionNode.DefaultInitializer"/> attribute must be computed.</remarks>
    public static partial IFixedSet<ITypeMemberDefinitionNode> ClassDefinition_SynthesizedMembers(IClassDefinitionNode node)
        // TODO should implicit self be included? (It isn't currently a type member)
        => FixedSet.CreateFromValue(node.DefaultInitializer);

    public static partial IDefaultInitializerDefinitionNode? StructDefinition_DefaultInitializer(IStructDefinitionNode node)
    {
        if (node.SourceMembers.Any(m => m is IInitializerDefinitionNode)) return null;
        return Child.Attach(node, IDefaultInitializerDefinitionNode.Create());
    }

    /// <remarks>This needs to be lazy computed because the
    /// <see cref="IStructDefinitionNode.DefaultInitializer"/> attribute must be computed.</remarks>
    public static partial IFixedSet<ITypeMemberDefinitionNode> StructDefinition_SynthesizedMembers(IStructDefinitionNode node)
        // TODO should implicit self be included? (It isn't currently a type member)
        => FixedSet.CreateFromValue(node.DefaultInitializer);

    public static partial IDefaultInitializerDefinitionNode? ValueDefinition_DefaultInitializer(IValueDefinitionNode node)
    {
        if (node.SourceMembers.Any(m => m is IInitializerDefinitionNode)) return null;
        return Child.Attach(node, IDefaultInitializerDefinitionNode.Create());
    }

    /// <remarks>This needs to be lazy computed because the
    /// <see cref="IValueDefinitionNode.DefaultInitializer"/> attribute must be computed.</remarks>
    public static partial IFixedSet<ITypeMemberDefinitionNode> ValueDefinition_SynthesizedMembers(IValueDefinitionNode node)
        // TODO should implicit self be included? (It isn't currently a type member)
        => FixedSet.CreateFromValue(node.DefaultInitializer);
    #endregion
}
