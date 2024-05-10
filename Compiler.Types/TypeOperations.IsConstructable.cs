using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    public static bool IsConstructable(this GenericParameterArgument genericParameterArgument)
    {
        var (param, arg) = genericParameterArgument;

        return Check(param, arg);

        static bool Check(GenericParameter param, DataType arg)
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
                case CapabilityViewpointType viewpointType:
                    // TODO handle capabilities on the generic parameter
                    if (!param.Constraint.IsAssignableFrom(viewpointType.Capability))
                        return false;
                    break;
                case SelfViewpointType selfViewpointType:
                    // TODO handle capabilities on the referent
                    if (!param.Constraint.IsAssignableFrom(selfViewpointType.Capability))
                        return false;
                    break;
                case EmptyType _:
                case UnknownType _:
                case FunctionType _:
                case ConstValueType _:
                    // ignore
                    break;
                default:
                    throw ExhaustiveMatch.Failed(arg);
            }
            return true;
        }
    }
}
