using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class MethodSelfParameterSyntax : ParameterSyntax, IMethodSelfParameterSyntax
{
    public bool IsLentBinding { get; }
    public ICapabilityConstraintSyntax Capability { get; }
    public Promise<SelfParameterSymbol> Symbol { get; } = new Promise<SelfParameterSymbol>();
    public override IPromise<Pseudotype> DataType { get; }

    public MethodSelfParameterSyntax(TextSpan span, bool isLentBinding,
        ICapabilityConstraintSyntax capability)
        : base(span, null)
    {
        Capability = capability;
        IsLentBinding = isLentBinding;
        DataType = Symbol.Select(s => s.Type);
    }

    public override string ToString()
    {
        var lent = IsLentBinding ? "lent " : "";
        return $"{lent}{Capability} self";
    }
}
