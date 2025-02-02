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
    /// Whether a parameter of this type be marked `lent`.
    /// </summary>
    /// <remarks>In general types that aren't `id` or `const` can be lent. However, types with
    /// independent parameters act almost like multiple parameters. So they can be lent if any of
    /// the independent parameter arguments can be lent. In that case, the `lent` keyword applies to
    /// any parameters that can be lent.</remarks>
    public static bool CanBeLent(this IMaybeType type)
        => type switch
        {
            CapabilityType t => t.Capability.CanBeLent() || t.BareType.ArgumentsCanBeLent(),
            OptionalType t => t.Referent.CanBeLent(),
            RefType t => t.IsMutableBinding || t.Referent.CanBeLent(),
            CapabilityViewpointType t => t.Capability.CanBeLent() && t.Referent.CanBeLent(),
            SelfViewpointType t => t.CapabilitySet.CanBeLent() && t.Referent.CanBeLent(),
            CapabilitySetSelfType t => t.CapabilitySet.CanBeLent() || t.BareType.ArgumentsCanBeLent(),
            GenericParameterType _ => true,
            VoidType _ => false,
            NeverType _ => false,
            UnknownType _ => false,
            FunctionType _ => false,
            _ => throw ExhaustiveMatch.Failed(type),
        };

    public static bool CanBeLent(this TypeParameterArgument arg)
        => (arg.Parameter.HasIndependence && arg.Argument.CanBeLent())
           || arg.Argument.ArgumentsCanBeLent();

    public static bool ArgumentsCanBeLent(this BareType type)
        => (type.HasIndependentTypeArguments && type.TypeParameterArguments.Any(a => a.CanBeLent()))
           || (type.ContainingType?.ArgumentsCanBeLent() ?? false);

    public static bool ArgumentsCanBeLent(this Type type)
        => type switch
        {
            CapabilityType t => t.BareType.ArgumentsCanBeLent(),
            OptionalType t => t.Referent.ArgumentsCanBeLent(),
            RefType t => t.IsMutableBinding || t.Referent.CanBeLent(),
            CapabilityViewpointType _ => false,
            // TODO is it right that the capability must be lendable? If so, explain why
            SelfViewpointType t => t.CapabilitySet.CanBeLent() && t.Referent.ArgumentsCanBeLent(),
            CapabilitySetSelfType t => false,
            GenericParameterType _ => false,
            VoidType _ => false,
            NeverType _ => false,
            FunctionType _ => false,
            _ => throw ExhaustiveMatch.Failed(type),
        };
}
