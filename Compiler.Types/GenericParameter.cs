using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A generic parameter to a type.
/// </summary>
/// <remarks>At the moment all generic parameters are types. In the future, they don't have to be.
/// That is why this class exists to ease refactoring later.</remarks>
public sealed class GenericParameter : IEquatable<GenericParameter>
{
    public GenericParameter(StandardTypeName name)
    {
        Requires.That(nameof(name), name.GenericParameterCount == 0, "Cannot have generic parameters");
        Name = name;
    }

    public StandardTypeName Name { get; }

    // TODO When parameters can be values not just types, add: public DataType DataType { get; }

    #region Equals
    public bool Equals(GenericParameter? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name.Equals(other.Name);
    }

    public override bool Equals(object? obj)
        => obj is GenericParameter other && Equals(other);

    public override int GetHashCode() => Name.GetHashCode();

    public static bool operator ==(GenericParameter? left, GenericParameter? right) => Equals(left, right);

    public static bool operator !=(GenericParameter? left, GenericParameter? right) => !Equals(left, right);
    #endregion

    public static implicit operator GenericParameter(string name) => new(name);

    public override string ToString() => Name.ToString();
}
