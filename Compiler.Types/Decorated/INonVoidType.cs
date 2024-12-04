using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[Closed(
    typeof(CapabilitySetSelfType),
    typeof(CapabilityType),
    typeof(FunctionType),
    typeof(GenericParameterType),
    typeof(OptionalType),
    typeof(SelfViewpointType),
    typeof(NeverType))]
public interface INonVoidType : IType
{
    new INonVoidPlainType PlainType { get; }
    IPlainType IType.PlainType => PlainType;
}
