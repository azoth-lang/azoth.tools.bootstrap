using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class FieldSymbolNode : PackageFacetChildSymbolNode, IFieldSymbolNode
{
    public override FieldSymbol Symbol { get; }
    public override IdentifierName Name => Symbol.Name;
    TypeName INamedDeclarationNode.Name => Name;
    public DataType BindingType => Symbol.Type;

    public FieldSymbolNode(FieldSymbol symbol)
    {
        Symbol = symbol;
    }
}
