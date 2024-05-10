using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class PackageMemberDeclarationNode : DeclarationNode, IPackageMemberDeclarationNode
{
    public abstract override IPackageMemberSymbolNode SymbolNode { get; }
    public abstract Symbol Symbol { get; }
}
