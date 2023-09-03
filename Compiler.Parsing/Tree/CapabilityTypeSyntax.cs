using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class CapabilityTypeSyntax : TypeSyntax, ICapabilityTypeSyntax
{
    public IReferenceCapabilitySyntax Capability { get; }
    public ITypeSyntax ReferentType { get; }

    public CapabilityTypeSyntax(
        IReferenceCapabilitySyntax referenceCapability,
        ITypeSyntax referentType,
        TextSpan span)
        : base(span)
    {
        ReferentType = referentType;
        Capability = referenceCapability;
    }

    public override string ToString() => $"{Capability} {ReferentType}";
}
