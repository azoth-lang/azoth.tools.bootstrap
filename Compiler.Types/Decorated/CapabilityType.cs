using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

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

    public IFixedList<IType> TypeArguments { get; }

    public CapabilityType(
        Capability capability,
        ConstructedOrVariablePlainType plainType,
        IFixedList<IType> typeArguments)
    {
        Requires.That(plainType.TypeArguments.SequenceEqual(typeArguments.Select(a => a.PlainType)), nameof(typeArguments),
            "Type arguments must match plain type.");
        Capability = capability;
        PlainType = plainType;
        TypeArguments = typeArguments;
    }

    public CapabilityType(Capability capability, ConstructedPlainType plainType)
    {
        Requires.That(plainType.TypeArguments.IsEmpty,
            nameof(plainType), "Plain type must have no type arguments.");
        Capability = capability;
        PlainType = plainType;
        TypeArguments = [];
    }

    #region Equality
    public bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilityType otherType
               && Capability.Equals(otherType.Capability)
               && PlainType.Equals(otherType.PlainType)
               && TypeArguments.Equals(otherType.TypeArguments);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is CapabilityType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Capability, PlainType, TypeArguments);
    #endregion

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString()
        => ToString(Capability.ToSourceCodeString(), t => t.ToSourceCodeString());

    public string ToILString()
        => ToString(Capability.ToILString(), t => t.ToILString());

    private string ToString(string capability, Func<IType, string> toString)
    {
        // TODO when the referent is a generic parameter type, this becomes a viewpoint type
        var builder = new StringBuilder();
        builder.Append(capability);
        builder.Append(' ');
        builder.Append(PlainType.ToBareString());
        if (!TypeArguments.IsEmpty)
        {
            builder.Append('[');
            builder.AppendJoin(", ", TypeArguments.Select(toString));
            builder.Append(']');
        }

        return builder.ToString();
    }
}
