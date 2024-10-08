using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

/// <summary>
/// A generic parameter definition for an antetype.
/// </summary>
public sealed class AntetypeGenericParameter : IEquatable<AntetypeGenericParameter>
{
    public IdentifierName Name { get; }

    public TypeVariance Variance { get; }

    public AntetypeGenericParameter(
        IdentifierName name,
        TypeVariance variance)
    {
        Variance = variance;
        Name = name;
    }

    #region Equality
    public bool Equals(AntetypeGenericParameter? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name.Equals(other.Name) && Variance == other.Variance;
    }

    public override bool Equals(object? obj)
        => obj is AntetypeGenericParameter other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Name, Variance);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append(Name);
        var variance = Variance.ToSourceCodeString();
        if (variance.Length != 0) builder.Append(' ').Append(variance);
        return builder.ToString();
    }
}
