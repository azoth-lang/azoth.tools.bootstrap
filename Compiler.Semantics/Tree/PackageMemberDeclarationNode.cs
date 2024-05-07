using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class PackageMemberDeclarationNode : DeclarationNode, IPackageMemberDeclarationNode
{
    public abstract override IPackageMemberSymbolNode SymbolNode { get; }
}
