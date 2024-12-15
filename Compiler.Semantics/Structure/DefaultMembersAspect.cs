using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class DefaultMembersAspect
{
    #region Type Definitions
    public static partial IImplicitSelfDefinitionNode TypeDefinition_ImplicitSelf(ITypeDefinitionNode node)
        => IImplicitSelfDefinitionNode.Create();

    public static partial IDefaultConstructorDefinitionNode? ClassDefinition_DefaultConstructor(IClassDefinitionNode node)
    {
        if (node.SourceMembers.Any(m => m is IConstructorDefinitionNode))
            return null;

        return Child.Attach(node, IDefaultConstructorDefinitionNode.Create());
    }

    /// <remarks>This needs to be lazy computed because the
    /// <see cref="IClassDefinitionNode.DefaultConstructor"/> attribute must be computed.</remarks>
    public static partial IFixedSet<IClassMemberDefinitionNode> ClassDefinition_Members(IClassDefinitionNode node)
    {
        var members = node.SourceMembers.AsEnumerable();

        var defaultConstructor = node.DefaultConstructor;
        if (defaultConstructor is not null)
            members = members.Append(defaultConstructor);

        return members.ToFixedSet();
    }

    public static partial IDefaultInitializerDefinitionNode? StructDefinition_DefaultInitializer(IStructDefinitionNode node)
    {
        if (node.SourceMembers.Any(m => m is IInitializerDefinitionNode))
            return null;

        return Child.Attach(node, IDefaultInitializerDefinitionNode.Create());
    }

    public static partial IFixedSet<IStructMemberDefinitionNode> StructDefinition_Members(IStructDefinitionNode node)
    {
        var members = node.SourceMembers.AsEnumerable();

        var defaultInitializer = node.DefaultInitializer;
        if (defaultInitializer is not null)
            members = members.Append(defaultInitializer);

        return members.ToFixedSet();
    }
    #endregion
}
