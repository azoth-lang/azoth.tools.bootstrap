using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

public class ReferenceCapabilityAssignmentTestCase
{
    public Capability From { get; }
    public Capability To { get; }
    public bool Assignable { get; }

    public ReferenceCapabilityAssignmentTestCase(
        Capability from,
        Capability to,
        bool assignable)
    {
        From = from;
        To = to;
        Assignable = assignable;
    }

    public override string ToString() => Assignable ? $"{From} <: {To}" : $"{From} ≮: {To}";
}
