using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(
    typeof(CapabilityType),
    typeof(GenericParameterType),
    typeof(ViewpointType),
    typeof(FunctionType),
    typeof(OptionalType),
    typeof(NeverType))]
public interface INonVoidType : IType, IMaybeNonVoidType
{
    public new INonVoidAntetype ToAntetype();
    IAntetype IType.ToAntetype() => ToAntetype();
    IMaybeAntetype IMaybeType.ToAntetype() => ToAntetype();
}
