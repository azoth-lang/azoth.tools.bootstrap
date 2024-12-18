using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
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

    internal abstract GenericParameterTypeReplacements BareTypeReplacements { get; }

    public TypeReplacements TypeReplacements
        => Lazy.Initialize(ref typeReplacements, this, static type => new(type));
    private TypeReplacements? typeReplacements;

    public override NonVoidType ToNonLiteral() => this;
}
