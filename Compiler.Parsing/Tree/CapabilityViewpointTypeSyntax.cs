using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class CapabilityViewpointTypeSyntax : TypeSyntax, ICapabilityViewpointTypeSyntax
{
    public ICapabilitySyntax Capability { get; }
    public ITypeSyntax Referent { get; }

    public CapabilityViewpointTypeSyntax(ICapabilitySyntax capability, ITypeSyntax referent)
        : base(TextSpan.Covering(capability.Span, referent.Span))
    {
        Capability = capability;
        Referent = referent;
    }

    public override string ToString() => $"{Capability}|>{Referent}";
}
