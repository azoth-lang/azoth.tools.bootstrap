using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The two empty types are `never` and `void`. They are the only types with
/// no values.
/// </summary>
[Closed(
    typeof(VoidType),
    typeof(NeverType))]
public abstract class EmptyType : Type, IType
{
    public SpecialTypeName Name { get; }

    public override bool IsEmpty => true;

    public override bool IsFullyKnown => true;

    private protected EmptyType(SpecialTypeName name)
    {
        Name = name;
    }

    public abstract override EmptyAntetype ToAntetype();
    IMaybeAntetype IMaybeType.ToAntetype() => ToAntetype();

    public override string ToSourceCodeString() => Name.ToString();

    public override string ToILString() => ToSourceCodeString();

    #region Equals
    public override bool Equals(IMaybeExpressionType? other)
        // Empty types are all fixed instances, so a reference equality suffices
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(Name);
    #endregion
}
