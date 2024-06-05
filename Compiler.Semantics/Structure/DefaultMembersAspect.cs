using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class DefaultMembersAspect
{
    public static IDefaultConstructorDefinitionNode? ClassDeclaration_DefaultConstructor(IClassDefinitionNode node)
    {
        if (node.SourceMembers.Any(m => m is IConstructorDefinitionNode))
            return null;

        return Child.Attach(node, new DefaultConstructorDefinitionNode());
    }

    /// <remarks>This needs to be lazy computed because the
    /// <see cref="IClassDefinitionNode.DefaultConstructor"/> attribute must be computed.</remarks>
    public static IFixedSet<IClassMemberDefinitionNode> ClassDeclaration_Members(IClassDefinitionNode node)
    {
        var members = node.SourceMembers.AsEnumerable();

        var defaultConstructor = node.DefaultConstructor;
        if (defaultConstructor is not null)
            members = members.Prepend(defaultConstructor);

        return members.ToFixedSet();
    }
}
