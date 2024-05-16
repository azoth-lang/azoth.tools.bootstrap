using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticChildSymbolNode : ChildNode, IChildDeclarationNode
{
    public override ISyntax? Syntax => null;
    public abstract Symbol Symbol { get; }
}
