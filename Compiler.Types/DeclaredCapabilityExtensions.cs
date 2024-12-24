using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static class DeclaredCapabilityExtensions
{
    [return: NotNullIfNotNull(nameof(defaultCapability))]
    public static Capability? ToCapability(this DeclaredCapability capability, Capability? defaultCapability)
        => capability switch
        {
            DeclaredCapability.Isolated => Capability.Isolated,
            DeclaredCapability.TemporarilyIsolated => Capability.TemporarilyIsolated,
            DeclaredCapability.Mutable => Capability.Mutable,
            DeclaredCapability.Default => defaultCapability,
            DeclaredCapability.Read => Capability.Read,
            DeclaredCapability.Constant => Capability.Constant,
            DeclaredCapability.TemporarilyConstant => Capability.TemporarilyConstant,
            DeclaredCapability.Identity => Capability.Identity,
            _ => throw ExhaustiveMatch.Failed(capability),
        };

    public static Capability ToCapabilityFor(this DeclaredCapability capability, BareTypeConstructor typeConstructor)
        => capability.ToCapability(typeConstructor.DefaultCapability);

    public static Capability ToSelfParameterCapability(this DeclaredCapability capability)
        => capability switch
        {
            DeclaredCapability.Mutable => Capability.InitMutable,
            DeclaredCapability.Isolated => Capability.InitMutable,
            DeclaredCapability.TemporarilyIsolated => Capability.InitMutable,
            DeclaredCapability.Constant => Capability.InitReadOnly,
            DeclaredCapability.Read => Capability.InitReadOnly,
            DeclaredCapability.Default => Capability.InitReadOnly,
            DeclaredCapability.TemporarilyConstant => Capability.InitReadOnly,
            DeclaredCapability.Identity => Capability.InitReadOnly,
            _ => throw ExhaustiveMatch.Failed(capability)
        };

    public static string ToSourceCodeString(this DeclaredCapability capability)
        => capability.ToCapability(null)?.ToSourceCodeString() ?? "";
}
