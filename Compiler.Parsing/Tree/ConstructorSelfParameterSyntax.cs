using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class MethodSelfParameterSyntax : ParameterSyntax, IMethodSelfParameterSyntax
{
    public bool IsLentBinding { get; }
    public ICapabilityConstraintSyntax Capability { get; }

    public MethodSelfParameterSyntax(TextSpan span, bool isLentBinding,
        ICapabilityConstraintSyntax capability)
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
