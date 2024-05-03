using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticTypeSymbolNode : SemanticChildSymbolNode, ITypeSymbolNode
{
    public abstract override UserTypeSymbol Symbol { get; }
    public abstract IFixedList<ITypeMemberSymbolNode> Members { get; }
}
