using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

[Closed(typeof(Capability), typeof(CapabilitySet))]
public interface ICapabilityConstraint
{
    bool AnyCapabilityAllowsWrite { get; }
    bool IsAssignableFrom(ICapabilityConstraint from);
    public string ToILString();
    public string ToSourceCodeString();
}
