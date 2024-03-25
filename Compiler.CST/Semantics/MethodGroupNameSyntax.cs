using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public class MethodGroupNameSyntax : SyntaxSemantics, IMemberAccessSyntaxSemantics
{
    public new IFixedSet<MethodSymbol> Symbols { get; }
    public Promise<MethodSymbol?> Symbol { get; } = new();
    IPromise<Symbol?> IMemberAccessSyntaxSemantics.Symbol => Symbol;
    public override IPromise<FunctionType?> Type { get; }

    public MethodGroupNameSyntax(IFixedSet<MethodSymbol> symbols)
        : base(symbols)
    {
        Symbols = symbols;
        Type = Symbol.Select(s => s?.MethodGroupType);
    }
}
