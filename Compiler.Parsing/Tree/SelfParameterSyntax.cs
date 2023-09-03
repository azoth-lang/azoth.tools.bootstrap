using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class SelfParameterSyntax : ParameterSyntax, ISelfParameterSyntax
{
    public bool IsMutableBinding => false;
    // TODO replace with a reference capability
    public bool MutableSelf { get; }
    public Promise<SelfParameterSymbol> Symbol { get; } = new Promise<SelfParameterSymbol>();
    IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
    public override IPromise<DataType> DataType { get; }
    public SelfParameterSyntax(TextSpan span, bool mutableSelf)
        : base(span, null)
    {
        MutableSelf = mutableSelf;
        DataType = Symbol.Select(s => s.DataType);
    }

    public override string ToString()
    {
        var value = "self";
        if (MutableSelf)
            value = "mut " + value;
        return value;
    }
}
