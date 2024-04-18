using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class Type : IEquatable<Type>
{
    public static IEqualityComparer<Type> EquivalenceComparer { get; }
        = EqualityComparer<Type>.Create((a, b) => a?.IsEquivalentTo(b) ?? false,
            t => HashCode.Combine(t.CollectionKind, t.IsOptional, t.Name, t.Symbol.IsExternal));

    public static Type Void { get; } = new Type(Symbol.Void, CollectionKind.None, isOptional: false);

    [return: NotNullIfNotNull(nameof(symbol))]
    public static Type? Create(Symbol? symbol, CollectionKind collectionKind = CollectionKind.None)
        => symbol is not null ? new Type(symbol, collectionKind, isOptional: false) : null;

    [return: NotNullIfNotNull(nameof(syntax))]
    public static Type? CreateFromSyntax(Grammar grammar, TypeNode? syntax)
        => syntax is not null ? new Type(grammar, syntax) : null;

    [return: NotNullIfNotNull(nameof(syntax))]
    public static Type? CreateExternalFromSyntax(TypeNode? syntax)
        => syntax is not null ? new Type(Symbol.CreateExternalFromSyntax(syntax.Symbol), syntax.CollectionKind, syntax.IsOptional) : null;

    public Symbol Symbol { get; }
    public string Name => Symbol.FullName;
    public CollectionKind CollectionKind { get; }
    public bool IsCollection => CollectionKind != CollectionKind.None;
    public bool IsOptional { get; }
    public Type UnderlyingType { get; }

    private Type(Grammar grammar, TypeNode syntax)
    {
        Symbol = Symbol.CreateFromSyntax(grammar, syntax.Symbol);
        CollectionKind = syntax.CollectionKind;
        IsOptional = syntax.IsOptional;
        UnderlyingType = CreateUnderlyingType();
    }

    private Type(Symbol symbol, CollectionKind collectionKind, bool isOptional)
    {
        Symbol = symbol;
        CollectionKind = collectionKind;
        IsOptional = isOptional;
        UnderlyingType = CreateUnderlyingType();
    }

    private Type CreateUnderlyingType()
    {
        if (!IsCollection) return this;
        return new Type(Symbol, CollectionKind.None, false);
    }

    public bool IsEquivalentTo(Type? other)
    {
        if (other is null)
            return false;
        return CollectionKind == other.CollectionKind
               && IsOptional == other.IsOptional
               && Name == other.Name
               && Symbol.IsExternal == other.Symbol.IsExternal;
    }

    #region Equality
    public bool Equals(Type? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return CollectionKind == other.CollectionKind
        && Symbol == other.Symbol;
    }

    public override bool Equals(object? obj) => obj is Type other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Symbol, (int)CollectionKind);

    public static bool operator ==(Type? left, Type? right) => Equals(left, right);

    public static bool operator !=(Type? left, Type? right) => !Equals(left, right);
    #endregion

    public override string ToString()
    {
        var type = Symbol.ToString();
        switch (CollectionKind)
        {
            default:
                throw ExhaustiveMatch.Failed(CollectionKind);
            case CollectionKind.None:
                // Nothing
                break;
            case CollectionKind.List:
                type += "*";
                break;
            case CollectionKind.Set:
                type = $"{{{type}}}";
                break;
        }

        if (IsOptional) type += "?";
        return type;
    }
}
