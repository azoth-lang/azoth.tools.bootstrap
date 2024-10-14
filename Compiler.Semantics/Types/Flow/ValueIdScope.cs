using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

/// <summary>
/// A scope or space of value IDs.
/// </summary>
/// <remarks>Generally each method/function body forms one scope. Field initializers also get their
/// own scope. Two <see cref="ValueId"/>s with the same number in a different scope are not equal.</remarks>
public sealed class ValueIdScope : IEquatable<ValueIdScope>
{
    private ulong current;

    public ValueId CreateNext()
    {
        // Increment returns the new value, but we want the previous value, so subtract 1.
        var newId = Interlocked.Increment(ref current) - 1;
        return new ValueId(this, newId);
    }

    #region Equality
    public bool Equals(ValueIdScope? other)
        // Equality is based on reference equality.
        => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        // Equality is based on reference equality.
        => ReferenceEquals(this, obj);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(ValueIdScope? left, ValueIdScope? right) => Equals(left, right);

    public static bool operator !=(ValueIdScope? left, ValueIdScope? right) => !Equals(left, right);
    #endregion
}
