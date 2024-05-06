using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class ReferencedFunctionSymbolNode : ReferencedDeclarationSymbolNode, IFunctionSymbolNode
{
    public override FunctionSymbol Symbol { get; }

    public ReferencedFunctionSymbolNode(FunctionSymbol symbol)
    {
        Symbol = symbol;
    }

    public override IEnumerable<IDeclarationSymbolNode> MembersNamed(IdentifierName named)
        => Enumerable.Empty<IDeclarationSymbolNode>();
}
