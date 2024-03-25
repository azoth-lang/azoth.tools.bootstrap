using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public sealed class InitializerGroupNameSyntax : SyntaxSemantics, IMemberAccessSyntaxSemantics
{
    public new IFixedSet<InitializerSymbol> Symbols { get; }
    public Promise<InitializerSymbol?> Symbol { get; } = new();
    IPromise<Symbol?> IMemberAccessSyntaxSemantics.Symbol => Symbol;
    public override IPromise<FunctionType?> Type { get; }

    public InitializerGroupNameSyntax(IFixedSet<InitializerSymbol> symbols)
        : base(symbols)
    {
        Symbols = symbols;
        Type = Symbol.Select(s => s?.InitializerGroupType);
    }
}
