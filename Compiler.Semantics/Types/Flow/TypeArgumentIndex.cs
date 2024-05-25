using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

/// <summary>
/// The index of a type argument in a type.
/// </summary>
/// <remarks>This is a tree based index where each number is the zero-based index of the type
/// argument at that level. For example, `1.0.2` is the third argument of the first argument of
/// the second argument of the outer type.</remarks>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public readonly struct TypeArgumentIndex : IEquatable<TypeArgumentIndex>
{
    public IFixedList<int> TreeIndex { get; }

    public TypeArgumentIndex(IFixedList<int> treeIndex)
    {
        TreeIndex = treeIndex;
    }

    #region Equality
    public bool Equals(TypeArgumentIndex other) => TreeIndex.Equals(other.TreeIndex);

    public override bool Equals(object? obj) => obj is TypeArgumentIndex other && Equals(other);

    public override int GetHashCode() => TreeIndex.GetHashCode();

    public static bool operator ==(TypeArgumentIndex left, TypeArgumentIndex right)
        => left.Equals(right);

    public static bool operator !=(TypeArgumentIndex left, TypeArgumentIndex right)
        => !left.Equals(right);
    #endregion

    public override string ToString() => string.Join('.', TreeIndex);
}
