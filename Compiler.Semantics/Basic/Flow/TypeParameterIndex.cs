using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// The index of a type parameter in a type.
/// </summary>
/// <remarks>This is a tree based index where each number is the zero-based index of the type
/// parameter at that level. For example, `1.0.2` is the third parameter of the first parameter of
/// the second parameter of the outer type.</remarks>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public readonly struct TypeParameterIndex : IEquatable<TypeParameterIndex>
{
    public FixedList<int> TreeIndex { get; }

    public TypeParameterIndex(FixedList<int> treeIndex)
    {
        TreeIndex = treeIndex;
    }

    #region Equality
    public bool Equals(TypeParameterIndex other) => TreeIndex.Equals(other.TreeIndex);

    public override bool Equals(object? obj) => obj is TypeParameterIndex other && Equals(other);

    public override int GetHashCode() => TreeIndex.GetHashCode();

    public static bool operator ==(TypeParameterIndex left, TypeParameterIndex right)
        => left.Equals(right);

    public static bool operator !=(TypeParameterIndex left, TypeParameterIndex right)
        => !left.Equals(right);
    #endregion

    public override string ToString() => string.Join('.', TreeIndex);
}
