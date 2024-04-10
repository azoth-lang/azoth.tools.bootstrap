using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;

public sealed class SymbolBuilderContext(Diagnostics diagnostics, ISymbolTreeBuilder symbolTree, ISymbolTreeBuilder testingSymbolTree)
{
    public Diagnostics Diagnostics { get; } = diagnostics;
    public ISymbolTreeBuilder SymbolTree { get; } = symbolTree;
    public ISymbolTreeBuilder TestingSymbolTree { get; } = testingSymbolTree;
}
