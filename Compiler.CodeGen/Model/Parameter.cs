using System;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class Parameter : IEquatable<Parameter>
{
    [return: NotNullIfNotNull(nameof(type))]
    public static Parameter? Create(NonVoidType? type, string name)
        => type is not null ? new Parameter(type, name) : null;

    [return: NotNullIfNotNull(nameof(syntax))]
    public static Parameter? CreateFromSyntax(Grammar? grammar, ParameterNode? syntax)
    {
        if (syntax is null) return null;
        var type = grammar is not null ? Types.Type.CreateFromSyntax(grammar, syntax.Type)
            : Types.Type.CreateExternalFromSyntax(syntax.Type);
        if (type is not NonVoidType nonVoidType)
            throw new InvalidOperationException("Parameter type cannot be void.");

        return new(nonVoidType, syntax.Name);

    }

    public NonVoidType Type { get; }
    public string Name { get; }
    public bool IsChildParameter => Name.StartsWith("child", StringComparison.Ordinal);
    public Parameter ChildParameter => childParameter.Value;
    private readonly Lazy<Parameter> childParameter;

    private Parameter(NonVoidType type, string name)
    {
        Type = type;
        Name = name;
        childParameter = new(() => IsChildParameter ? this : new(type, "child" + name.ToPascalCase()));
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

    public Parameter WithType(NonVoidType type) => type == Type ? this : new(type, Name);

    public override string ToString() => $"{Type} {Name}";
}
