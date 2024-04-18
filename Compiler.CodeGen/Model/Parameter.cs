using System;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Type = Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types.Type;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class Parameter : IEquatable<Parameter>
{
    public static Parameter Void { get; } = new Parameter(Type.Void, "_");

    [return: NotNullIfNotNull(nameof(type))]
    public static Parameter? Create(Type? type, string name)
        => type is not null ? new Parameter(type, name) : null;

    public ParameterNode? Syntax { get; }
    public Type Type { get; }
    public string Name { get; }

    public Parameter(Grammar? grammar, ParameterNode syntax)
    {
        Syntax = syntax;
        Type = new Type(grammar, syntax.Type);
        Name = syntax.Name;
    }

    private Parameter(Type type, string name)
    {
        Type = type;
        Name = name;
    }

    #region Equality
    public bool Equals(Parameter? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return Name == other.Name && Type == other.Type;
    }

    public override bool Equals(object? obj) => obj is Parameter other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Type, Name);

    public static bool operator ==(Parameter? left, Parameter? right) => Equals(left, right);

    public static bool operator !=(Parameter? left, Parameter? right) => !Equals(left, right);
    #endregion

    public override string ToString() => $"{Type} {Name}";
}
