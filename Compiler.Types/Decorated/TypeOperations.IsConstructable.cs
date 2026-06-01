using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    /// <summary>
    /// Check whether a given type parameter argument can validly be constructed.
    /// </summary>
    /// <remarks>This primarily checks that a generic arguments capability is compatible with the
    /// constraint on the type parameter. For this purpose, subtyping is not the correct thing.
    /// Rather the capability needs to be a subset or element of the constraint (e.g. a constraint
    /// of `read` can only be satisfied by a `read` type, not a `mut` type even thought `mut` is a
    /// subtype of `read`.</remarks></remarks>
    public static bool IsConstructable(this TypeParameterArgument self)
    {
        var (param, arg) = self;

        return Check(param, arg);

        static bool Check(TypeConstructorParameter param, Type arg)
        {
            switch (arg)
            {
                case CapabilityType capabilityType:
                    if (!capabilityType.Capability.IsSubsetOf(param.Constraint))
                        return false;
                    break;
                case OptionalType optionalType:
                    return Check(param, optionalType.Referent);
                case GenericParameterType genericParameterType:
                    if (!genericParameterType.ImplicitConstraint.IsSubsetOf(param.Constraint))
                        return false;
                    break;
                case CapabilityViewpointType viewpointType:
                    // TODO handle capabilities on the generic parameter
                    if (!viewpointType.Capability.IsSubsetOf(param.Constraint))
                        return false;
                    break;
                case CapabilitySetSelfType capabilitySetSelfType:
                    if (!capabilitySetSelfType.CapabilitySet.IsSubsetOf(param.Constraint))
                        return false;
                    break;
                case SelfViewpointType selfViewpointType:
                    // TODO handle capabilities on the referent
                    if (!selfViewpointType.CapabilitySet.IsSubsetOf(param.Constraint))
                        return false;
                    break;
                case CapabilitySetRestrictedType t:
                    if (!t.CapabilitySet.IsSubsetOf(param.Constraint))
                        return false;
                    break;
                case NeverType _:
                case VoidType _:
                case FunctionType _:
                    // ignore
                    break;
                default:
                    throw ExhaustiveMatch.Failed(arg);
            }
            return true;
        }
    }
}
