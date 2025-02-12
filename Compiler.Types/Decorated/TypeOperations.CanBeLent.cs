using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    public static bool CanBeLent(this Capability capability)
        => capability != Capability.Identity && capability != Capability.Constant;

    public static bool CanBeLent(this CapabilitySet capability)
        // TODO is this right? If so, explain why
        => capability == CapabilitySet.Readable;

    /// <summary>
    /// Whether a parameter of this type can be marked `lent`.
    /// </summary>
    /// <remarks>In general types that aren't `id` or `const` can be lent. However, types with
    /// independent parameters act almost like multiple parameters. So they can be lent if any of
    /// the independent parameter arguments can be lent. In that case, the `lent` keyword applies to
    /// any parameters that can be lent.</remarks>
    public static bool CanBeLent(this IMaybeType type)
        => type switch
        {
            CapabilityType t => t.Capability.CanBeLent() || t.BareType.GenericArgumentsCanBeLent(),
            OptionalType t => t.Referent.CanBeLent(),
            RefType t => t.IsMutableBinding || t.Referent.CanBeLent(),
            CapabilityViewpointType t => t.Capability.CanBeLent() && t.Referent.CanBeLent(),
            SelfViewpointType t => t.CapabilitySet.CanBeLent() && t.Referent.CanBeLent(),
            CapabilitySetSelfType t => t.CapabilitySet.CanBeLent() || t.BareType.GenericArgumentsCanBeLent(),
            CapabilitySetRestrictedType t => t.CapabilitySet.CanBeLent(),
            GenericParameterType _ => true,
            VoidType _ => false,
            NeverType _ => false,
            UnknownType _ => false,
            FunctionType _ => false,
            _ => throw ExhaustiveMatch.Failed(type),
        };

    public static bool CanBeLent(this TypeParameterArgument arg)
        => (arg.Parameter.HasIndependence && arg.Argument.CanBeLent())
           || arg.Argument.GenericArgumentsCanBeLent();

    public static bool GenericArgumentsCanBeLent(this BareType type)
        => (type.HasIndependentTypeArguments && type.TypeParameterArguments.Any(a => a.CanBeLent()))
           || (type.ContainingType?.GenericArgumentsCanBeLent() ?? false);

    public static bool GenericArgumentsCanBeLent(this Type type)
        => type switch
        {
            CapabilityType t => t.BareType.GenericArgumentsCanBeLent(),
            OptionalType t => t.Referent.GenericArgumentsCanBeLent(),
            RefType t => t.IsMutableBinding || t.Referent.CanBeLent(),
            CapabilityViewpointType _ => false, // No generic arguments
            // TODO is it right that the capability must be lendable? If so, explain why
            SelfViewpointType t => t.CapabilitySet.CanBeLent() && t.Referent.GenericArgumentsCanBeLent(),
            CapabilitySetSelfType _ => false, // No generic arguments
            CapabilitySetRestrictedType _ => false, // No generic arguments
            GenericParameterType _ => false, // No generic arguments
            VoidType _ => false, // No generic arguments
            NeverType _ => false, // No generic arguments
            FunctionType _ => false, // No generic arguments
            _ => throw ExhaustiveMatch.Failed(type),
        };
}
