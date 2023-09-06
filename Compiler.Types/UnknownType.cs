using System;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The type of expressions and values whose type could not be determined or
/// was somehow invalid. The unknown type can't be directly used in code.
/// No well typed program contains any value of the unknown type.
/// </summary>
public sealed class UnknownType : DataType
{
    #region Singleton
    internal static readonly UnknownType Instance = new();

    private UnknownType() { }
    #endregion

    public override bool IsKnown => false;

    /// <summary>
    /// Like `never` values of type unknown can be assigned to any value.
    /// It acts like a bottom type in this respect.
    /// </summary>
    public override TypeSemantics Semantics => TypeSemantics.Never;

    /// <remarks><see cref="ToSourceCodeString"/> is used to format error messages. As such, it
    /// is necessary to provide some output for the unknown type in case it appears in an error
    /// message.</remarks>
    public override string ToSourceCodeString() => "⧼unknown⧽";

    public override string ToILString() => "⧼unknown⧽";

    public override bool Equals(DataType? other)
        // The unknown type is a singleton, so reference equality suffices
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(UnknownType));
}
