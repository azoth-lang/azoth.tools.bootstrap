using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

[Closed(typeof(Capability), typeof(CapabilitySet))]
public interface ICapabilityConstraint
{
    bool SomeCapabilityAllowsWrite { get; }
    bool IsSubsetOf(Capability other);
    bool IsSubsetOf(CapabilitySet other);
    bool IsSubtypeOf(Capability other);
    bool IsSubtypeOf(CapabilitySet other);
    string ToILString();
    string ToSourceCodeString();
}


public static class CapabilityConstraintExtensions
{
    /// <summary>
    /// Is this capability constraint a subtype of the given capability constraint?
    /// </summary>
    public static bool IsSubsetOf(this ICapabilityConstraint self, ICapabilityConstraint other)
        => other switch
        {
            Capability capability => self.IsSubsetOf(capability),
            CapabilitySet capabilitySet => self.IsSubsetOf(capabilitySet),
            _ => throw ExhaustiveMatch.Failed(other),
        };

    /// <summary>
    /// Is this capability constraint set a subset of the given capability constraint?
    /// </summary>
    public static bool IsSubtypeOf(this ICapabilityConstraint self, ICapabilityConstraint other)
        => other switch
        {
            Capability capability => self.IsSubtypeOf(capability),
            CapabilitySet capabilitySet => self.IsSubtypeOf(capabilitySet),
            _ => throw ExhaustiveMatch.Failed(other),
        };
}
