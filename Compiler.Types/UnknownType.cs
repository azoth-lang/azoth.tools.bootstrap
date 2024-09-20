using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The type of expressions and values whose type could not be determined or
/// was somehow invalid. The unknown type can't be directly used in code.
/// No well typed program contains any expression with an unknown type.
/// </summary>
public sealed class UnknownType : DataType, IMaybeFunctionType
{
    #region Singleton
    internal static readonly UnknownType Instance = new();

    private UnknownType() { }
    #endregion

    public override bool IsFullyKnown => false;

    public override IMaybeExpressionAntetype ToAntetype() => IAntetype.Unknown;

    #region Equals
    public override bool Equals(DataType? other)
        // The unknown type is a singleton, so reference equality suffices
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(UnknownType));
    #endregion

    /// <remarks><see cref="ToSourceCodeString"/> is used to format error messages. As such, it
    /// is necessary to provide some output for the unknown type in case it appears in an error
    /// message.</remarks>
    public override string ToSourceCodeString() => "⧼unknown⧽";

    public override string ToILString() => "⧼unknown⧽";
}
