using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

[Closed(typeof(ReferenceCapability), typeof(ReferenceCapabilityConstraint))]
public interface IReferenceCapabilityConstraint
{
    bool IsAssignableFrom(IReferenceCapabilityConstraint from);
    bool AllowsRead { get; }
    bool AllowsWrite { get; }
    bool AllowsWriteAliases { get; }
}
