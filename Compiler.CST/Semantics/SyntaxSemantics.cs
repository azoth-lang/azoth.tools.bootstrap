using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

public abstract class SyntaxSemantics : ISyntaxSemantics
{
    public IFixedSet<Symbol> Symbols { get; }
    public virtual IPromise<DataType?> Type => Promise.Null<DataType>();

    protected SyntaxSemantics(IFixedSet<Symbol> symbols)
    {
        Symbols = symbols;
    }

    protected SyntaxSemantics(Symbol symbol)
    {
        Symbols = FixedSet.Create(symbol);
    }
}
