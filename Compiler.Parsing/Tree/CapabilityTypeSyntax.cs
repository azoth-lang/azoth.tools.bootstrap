using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class CapabilityTypeSyntax : TypeSyntax, ICapabilityTypeSyntax
{
    public IReferenceCapabilitySyntax Capability { get; }
    public ITypeSyntax Referent { get; }

    public CapabilityTypeSyntax(
        IReferenceCapabilitySyntax referenceCapability,
        ITypeSyntax referent,
        TextSpan span)
        : base(span)
    {
        Referent = referent;
        Capability = referenceCapability;
    }

    public override string ToString() => $"{Capability} {Referent}";
}
