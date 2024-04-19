using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[Closed(
    typeof(NonVoidType),
    typeof(VoidType))]
public abstract class Type : IEquatable<Type>
{
    public static IEqualityComparer<Type> EquivalenceComparer { get; }
        = EqualityComparer<Type>.Create(AreEquivalent, t => t.GetEquivalenceHashCode());

    public static VoidType Void { get; } = VoidType.Instance;
    public static SymbolType VoidSymbol { get; } = new SymbolType(new ExternalSymbol("Void"));

    [return: NotNullIfNotNull(nameof(syntax))]
    public static Type? CreateFromSyntax(Grammar grammar, TypeNode? syntax)
    {
        if (syntax is null)
            return null;
        return CreateDerivedType(SymbolType.CreateFromSyntax(grammar, syntax.Symbol), syntax);
    }

    [return: NotNullIfNotNull(nameof(syntax))]
    public static Type? CreateExternalFromSyntax(TypeNode? syntax)
    {
        if (syntax is null)
            return null;
        return CreateDerivedType(SymbolType.CreateExternalFromSyntax(syntax.Symbol), syntax);
    }

    private static NonVoidType CreateDerivedType(SymbolType underlyingType, TypeNode syntax)
    {
        var type = syntax.CollectionKind switch
        {
            CollectionKind.List => new ListType(underlyingType),
            CollectionKind.Set => new SetType(underlyingType),
            CollectionKind.None => (NonVoidType)underlyingType,
            _ => throw ExhaustiveMatch.Failed(syntax.CollectionKind)
        };
        if (syntax.IsOptional) type = new OptionalType(type);
        return type;
    }

    private protected Type() { }

    #region Equality
    public abstract bool Equals(Type? other);

    public override bool Equals(object? obj) => obj is Type other && Equals(other);

    public abstract override int GetHashCode();

    public static bool operator ==(Type? left, Type? right) => Equals(left, right);

    public static bool operator !=(Type? left, Type? right) => !Equals(left, right);
    #endregion

    public static bool AreEquivalent(Type? a, Type? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return (a, b) switch
        {
            (ListType left, ListType right) => AreEquivalent(left.ElementType, right.ElementType),
            (SetType left, SetType right) => AreEquivalent(left.ElementType, right.ElementType),
            (OptionalType left, OptionalType right) => AreEquivalent(left.UnderlyingType, right.UnderlyingType),
            (SymbolType left, SymbolType right) => Symbol.AreEquivalent(left.Symbol, right.Symbol),
            (VoidType, VoidType) => true,
            _ => false
        };
    }

    public abstract int GetEquivalenceHashCode();

    public abstract Type WithSymbol(Symbol symbol);

    public abstract override string ToString();
}
