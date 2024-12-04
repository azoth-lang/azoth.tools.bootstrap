namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// TODO should this cover optional types since they are implicit const?
// e.g. `mut Foo`, `const Self`, etc. when not applied to GenericParameterPlainType
// e.g. `read |> T` when applied to GenericParameterPlainType
// Cannot be applied to FunctionPlainType, NeverPlainType
// Open Questions:
// * Can it be applied to `void` in which case it must be implicit `const`?
// * Can it be applied to optional types in which case it must be implicit `const`?
// If answer to both is no, then can apply to:
// * VariablePlainType
//   * GenericParameterPlainType
//   * SelfParameterPlainType
//   * AssociatedPlainType
// * ConstructedPlainType
public sealed class CapabilityType
{

}
