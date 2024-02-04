using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

public class ReferenceCapabilityAssignmentTestCase
{
    public ReferenceCapability From { get; }
    public ReferenceCapability To { get; }
    public bool Assignable { get; }

    public ReferenceCapabilityAssignmentTestCase(
        ReferenceCapability from,
        ReferenceCapability to,
        bool assignable)
    {
        From = from;
        To = to;
        Assignable = assignable;
    }

    public override string ToString() => Assignable ? $"{From} <: {To}" : $"{From} ≮: {To}";
}
