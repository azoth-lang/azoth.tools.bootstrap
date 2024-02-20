using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

/// <summary>
/// A generic parameter to a type.
/// </summary>
/// <remarks>At the moment all generic parameters are types. In the future, they don't have to be.
/// That is why this class exists to ease refactoring later.</remarks>
public sealed class GenericParameter : IEquatable<GenericParameter>
{
    public static GenericParameter Invariant(StandardTypeName name)
        => new(ParameterVariance.Invariant, name);

    public static GenericParameter Independent(StandardTypeName name)
        => new(ParameterVariance.Independent, name);

    public static GenericParameter Out(StandardTypeName name)
        => new(ParameterVariance.Covariant, name);

    public static GenericParameter In(StandardTypeName name)
        => new(ParameterVariance.Contravariant, name);

    public GenericParameter(ParameterVariance parameterVariance, StandardTypeName name)
    {
        Requires.That(nameof(name), name.GenericParameterCount == 0, "Cannot have generic parameters");
        ParameterVariance = parameterVariance;
        Name = name;
    }

    public ParameterVariance ParameterVariance { get; }

    public TypeVariance TypeVariance => ParameterVariance.ToTypeVariance();

    public bool IsIndependent => ParameterVariance == ParameterVariance.Independent;

    public StandardTypeName Name { get; }

    // TODO When parameters can be values not just types, add: public DataType DataType { get; }

    #region Equals
    public bool Equals(GenericParameter? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ParameterVariance == other.ParameterVariance && Name.Equals(other.Name);
    }

    public override bool Equals(object? obj)
        => obj is GenericParameter other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(ParameterVariance, Name);

    public static bool operator ==(GenericParameter? left, GenericParameter? right) => Equals(left, right);

    public static bool operator !=(GenericParameter? left, GenericParameter? right) => !Equals(left, right);
    #endregion

    public override string ToString()
        => ParameterVariance == ParameterVariance.Invariant ? Name.ToString() : $"{ParameterVariance.ToSourceCodeString()} {Name}";
}
