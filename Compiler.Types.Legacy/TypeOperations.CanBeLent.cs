using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public static partial class TypeOperations
{
    public static bool CanBeLent(this Capability capability)
        => capability != Capability.Identity && capability != Capability.Constant;

    public static bool CanBeLent(this CapabilitySet capability)
        => capability == CapabilitySet.Readable;

    public static bool CanBeLent(this IMaybePseudotype type)
        => type switch
        {
            CapabilityTypeConstraint t => t.CanBeLent(),
            IMaybeExpressionType t => t.CanBeLent(),
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public static bool CanBeLent(this CapabilityTypeConstraint type)
        => type.Capability.CanBeLent() || type.BareType.ArgumentsCanBeLent();

    public static bool CanBeLent(this IMaybeExpressionType type)
        => type switch
        {
            CapabilityType t => t.Capability.CanBeLent() || t.BareType.ArgumentsCanBeLent(),
            OptionalType t => t.Referent.CanBeLent(),
            SelfViewpointType t => t.Capability.CanBeLent() && t.Referent.CanBeLent(),
            CapabilityViewpointType t => t.Capability.CanBeLent() && t.Referent.CanBeLent(),
            GenericParameterType _ => true,
            EmptyType _ => false,
            UnknownType _ => false,
            FunctionType _ => false,
            ConstValueType _ => false,
            _ => throw ExhaustiveMatch.Failed(type),
        };

    public static bool CanBeLent(this GenericParameterArgument arg)
        => (arg.Parameter.HasIndependence && arg.Argument.CanBeLent())
           || arg.Argument.ArgumentsCanBeLent();

    public static bool ArgumentsCanBeLent(this BareNonVariableType type)
        => type.HasIndependentTypeArguments && type.GenericParameterArguments.Any(a => a.CanBeLent());

    public static bool ArgumentsCanBeLent(this IMaybeType type)
        => type switch
        {
            CapabilityType t => t.BareType.ArgumentsCanBeLent(),
            OptionalType t => t.Referent.ArgumentsCanBeLent(),
            SelfViewpointType t => t.Capability.CanBeLent() && t.Referent.ArgumentsCanBeLent(),
            CapabilityViewpointType _ => false,
            GenericParameterType _ => false,
            EmptyType _ => false,
            UnknownType _ => false,
            FunctionType _ => false,
            _ => throw ExhaustiveMatch.Failed(type),
        };
}
