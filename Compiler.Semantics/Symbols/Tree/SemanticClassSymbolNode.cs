using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticClassSymbolNode : SemanticUserTypeSymbolNode, IClassSymbolNode
{
    protected override IClassDefinitionNode Node { get; }
    private ValueAttribute<IFixedList<IClassMemberDeclarationNode>> members;
    public override IFixedList<IClassMemberDeclarationNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(Node, SymbolNodeAttributes.ClassDeclaration_MembersSymbolNodes);

    public SemanticClassSymbolNode(IClassDefinitionNode node)
    {
        Node = node;
    }
}
