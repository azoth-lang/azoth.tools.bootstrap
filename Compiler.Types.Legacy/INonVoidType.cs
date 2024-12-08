using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

[Closed(
    typeof(CapabilityType),
    typeof(GenericParameterType),
    typeof(ViewpointType),
    typeof(FunctionType),
    typeof(OptionalType),
    typeof(NeverType))]
public interface INonVoidType : IType, IMaybeNonVoidType
{
    public new INonVoidPlainType ToPlainType();
    IPlainType IType.ToPlainType() => ToPlainType();
    IMaybePlainType IMaybeType.ToPlainType() => ToPlainType();

    new Decorated.INonVoidType ToDecoratedType();
    Decorated.IType IType.ToDecoratedType() => ToDecoratedType();
}
