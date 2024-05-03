using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticTypeSymbolNode : SemanticDeclarationSymbolNode, ITypeSymbolNode
{
    protected abstract ITypeDeclarationNode Node { get; }
    public override StandardName Name => Node.Name;
    public abstract override UserTypeSymbol Symbol { get; }
    public abstract IFixedList<ITypeMemberSymbolNode> Members { get; }
}
