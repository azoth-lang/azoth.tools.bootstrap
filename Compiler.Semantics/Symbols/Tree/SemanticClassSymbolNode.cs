using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticClassSymbolNode : SemanticUserTypeSymbolNode, IClassSymbolNode
{
    protected override IClassDeclarationNode Node { get; }
    private ValueAttribute<IFixedList<IClassMemberSymbolNode>> members;
    public override IFixedList<IClassMemberSymbolNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(Node, SymbolNodeAttributes.ClassDeclaration_MembersSymbolNodes);

    public SemanticClassSymbolNode(IClassDeclarationNode node)
    {
        Node = node;
    }
}
