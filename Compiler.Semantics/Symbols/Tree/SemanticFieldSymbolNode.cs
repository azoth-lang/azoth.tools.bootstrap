using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticFieldSymbolNode : SemanticDeclarationSymbolNode, IFieldSymbolNode
{
    private IFieldDeclarationNode Node { get; }
    public override FieldSymbol Symbol => Node.Symbol;
    public override IdentifierName Name => Node.Name;
    StandardName INamedSymbolNode.Name => Node.Name;
    public DataType Type => Node.Type;

    public SemanticFieldSymbolNode(IFieldDeclarationNode node)
    {
        Node = node;
    }
}