using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class ReferencedDeclarationSymbolNode : ReferencedChildSymbolNode, IDeclarationSymbolNode
{
    public virtual StandardName Name => (StandardName)Symbol.Name!;
}