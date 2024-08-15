using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[Closed(typeof(NonVoidType))]
public abstract class Type : IEquatable<Type>
{
    [return: NotNullIfNotNull(nameof(syntax))]
    public static Type? CreateFromSyntax(TreeModel tree, TypeSyntax? syntax)
    {
        if (syntax is null)
            return null;
        return CreateDerivedType(SymbolType.CreateFromSyntax(tree, syntax.Symbol), syntax);
    }

    [return: NotNullIfNotNull(nameof(syntax))]
    public static Type? CreateExternalFromSyntax(TypeSyntax? syntax)
    {
        if (syntax is null)
            return null;
        return CreateDerivedType(SymbolType.CreateExternalFromSyntax(syntax.Symbol), syntax);
    }

    private static NonVoidType CreateDerivedType(SymbolType underlyingType, TypeSyntax syntax)
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

    public abstract Type WithSymbol(Symbol symbol);

    public abstract bool IsSubtypeOf(Type other);

    public abstract override string ToString();
}
