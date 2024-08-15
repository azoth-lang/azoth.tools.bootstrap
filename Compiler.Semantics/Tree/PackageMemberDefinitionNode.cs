using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class PackageMemberDefinitionNode : DefinitionNode, IPackageMemberDefinitionNode
{
    TypeName INamedDeclarationNode.Name => Name;
    public IFixedList<IAttributeNode> Attributes { get; }
    private ValueAttribute<AccessModifier> accessModifier;
    public AccessModifier AccessModifier
        => accessModifier.TryGetValue(out var value) ? value
            : accessModifier.GetValue(this, TypeModifiersAspect.PackageMemberDeclaration_AccessModifier);
    public abstract override StandardName Name { get; }

    private protected PackageMemberDefinitionNode(IEnumerable<IAttributeNode> attributes)
    {
        Attributes = ChildList.Attach(this, attributes);
    }
}
