using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Types;

public static class DeclaredCapabilitySetExtensions
{
    public static string ToSourceCodeString(this DeclaredCapabilitySet self)
        => self switch
        {
            DeclaredCapabilitySet.Readable => "readable",
            DeclaredCapabilitySet.Shareable => "shareable",
            DeclaredCapabilitySet.Aliasable => "aliasable",
            DeclaredCapabilitySet.Sendable => "sendable",
            DeclaredCapabilitySet.Temporary => "temporary",
            DeclaredCapabilitySet.Any => "any",
            _ => throw ExhaustiveMatch.Failed(self),
        };
}
