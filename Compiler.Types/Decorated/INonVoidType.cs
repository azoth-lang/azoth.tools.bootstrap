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
    typeof(CapabilityViewpointType),
    typeof(NeverType))]
// TODO consider making this a class
public interface INonVoidType : IType, IMaybeNonVoidType
{
    new NonVoidPlainType PlainType { get; }
    IPlainType IType.PlainType => PlainType;
    IMaybeNonVoidPlainType IMaybeNonVoidType.PlainType => PlainType;

    public TypeReplacements TypeReplacements { get; }
}
