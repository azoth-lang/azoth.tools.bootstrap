using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class PackageMemberDeclarationNode : DeclarationNode, IPackageMemberDeclarationNode
{
    public IFixedList<IAttributeNode> Attributes { get; }
    private ValueAttribute<AccessModifier> accessModifier;
    public AccessModifier AccessModifier
        => accessModifier.TryGetValue(out var value) ? value
            : accessModifier.GetValue(this, TypeModifiersAspect.PackageMemberDeclaration_AccessModifier);
    public abstract override IPackageMemberSymbolNode SymbolNode { get; }
    public abstract Symbol Symbol { get; }

    private protected PackageMemberDeclarationNode(IEnumerable<IAttributeNode> attributes)
    {
        Attributes = ChildList.CreateFixed(this, attributes);
    }
}
