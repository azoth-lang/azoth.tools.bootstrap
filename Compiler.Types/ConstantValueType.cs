using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The type of a constant value.
/// </summary>
public abstract class ConstantValueType : NonEmptyType
{
    public SpecialTypeName Name { get; }

    public override TypeSemantics Semantics => TypeSemantics.CopyValue;

    private protected ConstantValueType(SpecialTypeName name)
    {
        Name = name;
    }

    #region Equals
    public override bool Equals(DataType? other)
        // Most constant value types are fixed instances, so a reference comparision suffices
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(Name);
    #endregion

    public override string ToSourceCodeString() => Name.ToString();

    public override string ToILString() => ToSourceCodeString();
}
