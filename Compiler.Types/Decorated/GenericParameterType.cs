using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// TODO maybe this should only be constructed from the GenericParameterPlainType to avoid duplicate instances?
// e.g. T
// NOTE: generic parameters are the only plain types that do not need a capability
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class GenericParameterType : INonVoidType
{
    public GenericParameterPlainType PlainType { get; }
    INonVoidPlainType INonVoidType.PlainType => PlainType;
    IMaybePlainType IMaybeType.PlainType => PlainType;

    public TypeConstructor.Parameter Parameter => PlainType.Parameter;

    public TypeReplacements TypeReplacements => TypeReplacements.None;

    public bool HasIndependentTypeArguments => false;

    public GenericParameterType(GenericParameterPlainType plainType)
    {
        PlainType = plainType;
    }

    #region Equality
    public bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is GenericParameterType otherType
               && PlainType.Equals(otherType.PlainType);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is GenericParameterType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(PlainType);
    #endregion

    /// <remarks>There are cases where the framework calls <see cref="ToString"/> so it needs to map
    /// to something reasonable.</remarks>
    public override string ToString() => ToILString();

    public string ToSourceCodeString() => PlainType.ToString();

    public string ToILString() => PlainType.ToString();
}
