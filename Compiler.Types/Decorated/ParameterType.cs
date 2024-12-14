using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

/// <summary>
/// For function types it is necessary to know not only the type of each parameter, but whether it
/// was declared `lent`. This class packages those two values.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public record class ParameterType(bool IsLent, NonVoidType Type) : IMaybeParameterType
{
    public static IMaybeParameterType Create(bool isLent, IMaybeNonVoidType type)
    {
        if (type is NonVoidType t) return new ParameterType(isLent, t);
        return Decorated.Type.Unknown;
    }
    IMaybeNonVoidType IMaybeParameterType.Type => Type;

    public NonVoidPlainType PlainType => Type.PlainType;

    public bool ReferenceEquals(ParameterType other)
        => IsLent == other.IsLent && ReferenceEquals(Type, other.Type);

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => IsLent ? $"lent {Type.ToILString()}" : Type.ToILString();

    public string ToSourceCodeString()
        => IsLent ? $"lent {Type.ToSourceCodeString()}" : Type.ToSourceCodeString();
}
