using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    public static bool IsConstructable(this TypeParameterArgument self)
    {
        var (param, arg) = self;

        return Check(param, arg);

        static bool Check(TypeConstructor.Parameter param, IType arg)
        {
            switch (arg)
            {
                case CapabilityType capabilityType:
                    if (!param.Constraint.IsAssignableFrom(capabilityType.Capability))
                        return false;
                    break;
                case OptionalType optionalType:
                    return Check(param, optionalType.Referent);
                case GenericParameterType genericParameterType:
                    if (!param.Constraint.IsAssignableFrom(genericParameterType.Parameter.Constraint))
                        return false;
                    break;
                //case CapabilityViewpointType viewpointType:
                //    // TODO handle capabilities on the generic parameter
                //    if (!param.Constraint.IsAssignableFrom(viewpointType.Capability))
                //        return false;
                //    break;
                case CapabilitySetSelfType capabilitySetSelfType:
                    if (!param.Constraint.IsAssignableFrom(capabilitySetSelfType.Capability))
                        return false;
                    break;
                case SelfViewpointType selfViewpointType:
                    // TODO handle capabilities on the referent
                    if (!param.Constraint.IsAssignableFrom(selfViewpointType.Capability))
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
