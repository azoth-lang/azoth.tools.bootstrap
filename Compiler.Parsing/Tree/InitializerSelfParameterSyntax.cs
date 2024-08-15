using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class InitializerSelfParameterSyntax : ParameterSyntax, IInitializerSelfParameterSyntax
{
    public bool IsLentBinding { get; }
    public ICapabilitySyntax Capability { get; }

    public InitializerSelfParameterSyntax(TextSpan span, bool isLentBinding, ICapabilitySyntax capability)
        : base(span, null)
    {
        Capability = capability;
        IsLentBinding = isLentBinding;
    }

    public override string ToString()
    {
        var lent = IsLentBinding ? "lent " : "";
        return $"{lent}{Capability} self";
    }
}
