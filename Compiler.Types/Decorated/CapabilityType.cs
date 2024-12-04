using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

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
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class CapabilityType : INonVoidType
{
    public Capability Capability { get; }
    public ConstructedOrVariablePlainType PlainType { get; }
    INonVoidPlainType INonVoidType.PlainType => PlainType;
    // TODO represent the decoration on the plain type (type arguments should work)

    public CapabilityType(Capability capability, ConstructedOrVariablePlainType plainType)
    {
        Capability = capability;
        PlainType = plainType;
    }

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => $"{Capability.ToSourceCodeString()} {PlainType}";

    public string ToILString() => $"{Capability.ToILString()} {PlainType}";
}
