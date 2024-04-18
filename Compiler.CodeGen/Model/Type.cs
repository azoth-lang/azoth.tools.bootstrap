using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class Type : IEquatable<Type>
{
    public static IEqualityComparer<Type> EquivalenceComparer { get; }
        = EqualityComparer<Type>.Create((a, b) => a?.IsEquivalentTo(b) ?? false,
            t => HashCode.Combine(t.CollectionKind, t.IsOptional, t.Name, t.Symbol.Syntax.IsQuoted));

    public static Type Void { get; } = new Type(null, Symbol.Void, CollectionKind.None);

    [return: NotNullIfNotNull(nameof(symbol))]
    public static Type? Create(Grammar? grammar, Symbol? symbol, CollectionKind collectionKind = CollectionKind.None)
        => symbol is not null ? new Type(grammar, symbol, collectionKind) : null;

    public Language? Language { get; }
    public Grammar? Grammar { get; }
    public TypeNode? Syntax { get; }

    public Symbol Symbol { get; }
    public string Name => Symbol.Name;
    public CollectionKind CollectionKind { get; }
    public bool IsCollection => CollectionKind != CollectionKind.None;
    public bool IsOptional => Syntax?.IsOptional ?? false;
    public Type UnderlyingType { get; }

    public Type(Grammar? grammar, TypeNode syntax)
    {
        Language = grammar?.Language;
        Grammar = grammar;
        Syntax = syntax;
        Symbol = new Symbol(grammar, syntax.Symbol);
        CollectionKind = syntax.CollectionKind;
        UnderlyingType = CreateUnderlyingType();
    }

    private Type(Grammar? grammar, Symbol symbol, CollectionKind collectionKind)
    {
        Language = grammar?.Language;
        Grammar = grammar;
        Symbol = symbol;
        CollectionKind = collectionKind;
        UnderlyingType = CreateUnderlyingType();
    }

    private Type CreateUnderlyingType()
    {
        if (!IsCollection) return this;
        return new Type(Grammar, Symbol, CollectionKind.None);
    }

    public bool IsEquivalentTo(Type? other)
    {
        if (other is null)
            return false;
        return CollectionKind == other.CollectionKind
               && IsOptional == other.IsOptional
               && Name == other.Name
               && Symbol.Syntax.IsQuoted == other.Symbol.Syntax.IsQuoted;
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
