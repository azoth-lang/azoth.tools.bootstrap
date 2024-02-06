using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ConstructorSelfParameterSyntax : ParameterSyntax, IConstructorSelfParameterSyntax
{
    public bool IsLentBinding { get; }
    public IReferenceCapabilitySyntax Capability { get; }
    public Promise<SelfParameterSymbol> Symbol { get; } = new Promise<SelfParameterSymbol>();
    public override IPromise<DataType> DataType { get; }
    public ConstructorSelfParameterSyntax(TextSpan span, bool isLentBinding, IReferenceCapabilitySyntax capability)
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
