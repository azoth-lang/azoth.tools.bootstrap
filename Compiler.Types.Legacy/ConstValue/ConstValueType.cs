using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;

/// <summary>
/// The type of a constant value.
/// </summary>
[Closed(
    typeof(BoolConstValueType),
    typeof(IntegerConstValueType))]
public abstract class ConstValueType : NonEmptyType, INonVoidType
{
    public SpecialTypeName Name { get; }

    public virtual bool IsTypeOfConstValue => true;

    private protected ConstValueType(SpecialTypeName name)
    {
        Name = name;
    }

    #region Equals
    public override bool Equals(IMaybeType? other)
        // Most constant value types are fixed instances, so a reference comparision suffices
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(Name);
    #endregion

    public override string ToSourceCodeString() => Name.ToString();

    public override string ToILString() => ToSourceCodeString();
}
