using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    public static bool CanBeLent(this Capability capability)
        => capability != Capability.Identity && capability != Capability.Constant;

    public static bool CanBeLent(this CapabilitySet capability)
        => capability == CapabilitySet.Readable;

    public static bool CanBeLent(this Pseudotype type)
        => type switch
        {
            CapabilityTypeConstraint t => t.CanBeLent(),
            DataType t => t.CanBeLent(),
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public static bool CanBeLent(this CapabilityTypeConstraint type)
        => type.Capability.CanBeLent() || type.BareType.ArgumentsCanBeLent();

    public static bool CanBeLent(this DataType type)
        => type switch
        {
            // TODO combine into CapabilityType
            ReferenceType t => t.Capability.CanBeLent() || t.BareType.ArgumentsCanBeLent(),
            ValueType t => t.BareType.ArgumentsCanBeLent(),
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

    public static bool ArgumentsCanBeLent(this BareType type)
        => type.HasIndependentTypeArguments && type.GenericParameterArguments.Any(a => a.CanBeLent());

    public static bool ArgumentsCanBeLent(this DataType type)
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
            ConstValueType _ => false,
            _ => throw ExhaustiveMatch.Failed(type),
        };
}
