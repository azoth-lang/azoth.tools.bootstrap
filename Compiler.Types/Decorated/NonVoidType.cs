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
public abstract class NonVoidType : Type, IMaybeNonVoidType
{
    public abstract override NonVoidPlainType PlainType { get; }
    IMaybeNonVoidPlainType IMaybeNonVoidType.PlainType => PlainType;

    public abstract BareTypeReplacements TypeReplacements { get; }

    public override NonVoidType ToNonLiteral() => this;
}
