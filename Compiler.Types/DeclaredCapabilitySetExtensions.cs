using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static class DeclaredCapabilitySetExtensions
{
    public static CapabilitySet ToCapabilitySet(this DeclaredCapabilitySet self)
        => self switch
        {
            DeclaredCapabilitySet.Readable => CapabilitySet.Readable,
            DeclaredCapabilitySet.Shareable => CapabilitySet.Shareable,
            DeclaredCapabilitySet.Aliasable => CapabilitySet.Aliasable,
            DeclaredCapabilitySet.Sendable => CapabilitySet.Sendable,
            DeclaredCapabilitySet.ReadOnly => CapabilitySet.ReadOnly,
            DeclaredCapabilitySet.Temporary => CapabilitySet.Temporary,
            DeclaredCapabilitySet.Any => CapabilitySet.Any,
            _ => throw ExhaustiveMatch.Failed(self),
        };
}
