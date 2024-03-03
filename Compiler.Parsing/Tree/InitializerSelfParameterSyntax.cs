using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class InitializerSelfParameterSyntax : ParameterSyntax, IInitializerSelfParameterSyntax
{
    public bool IsLentBinding { get; }
    public ICapabilitySyntax Capability { get; }
    public Promise<SelfParameterSymbol> Symbol { get; } = new Promise<SelfParameterSymbol>();
    public override IPromise<DataType> DataType { get; }

    public InitializerSelfParameterSyntax(TextSpan span, bool isLentBinding, ICapabilitySyntax capability)
        : base(span, null)
    {
        Capability = capability;
        IsLentBinding = isLentBinding;
        DataType = Symbol.Select(s => (DataType)s.Type);
    }

    public override string ToString()
    {
        var lent = IsLentBinding ? "lent " : "";
        return $"{lent}{Capability} self";
    }
}
