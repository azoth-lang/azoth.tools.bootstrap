using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[Closed(typeof(NonOptionalType), typeof(OptionalType))]
public abstract class TypeModel : IEquatable<TypeModel>
{
    [return: NotNullIfNotNull(nameof(syntax))]
    public static TypeModel? CreateFromSyntax(TreeModel tree, TypeSyntax? syntax)
    {
        if (syntax is null)
            return null;
        return CreateDerivedType(SymbolType.CreateFromSyntax(tree, syntax.Symbol), syntax);
    }

    private static TypeModel CreateDerivedType(SymbolType underlyingType, TypeSyntax syntax)
    {
        var type = syntax.CollectionKind switch
        {
            CollectionKind.List => new ListType(underlyingType),
            CollectionKind.Set => new SetType(underlyingType),
            CollectionKind.None => (TypeModel)underlyingType,
            _ => throw ExhaustiveMatch.Failed(syntax.CollectionKind)
        };
        if (syntax.IsOptional) type = new OptionalType(type);
        return type;
    }

    public Symbol UnderlyingSymbol { get; }

    private protected TypeModel(Symbol underlyingSymbol)
    {
        UnderlyingSymbol = underlyingSymbol;
    }

    #region Equality
    public abstract bool Equals(TypeModel? other);

    public override bool Equals(object? obj) => obj is TypeModel other && Equals(other);

    public abstract override int GetHashCode();

    public static bool operator ==(TypeModel? left, TypeModel? right) => Equals(left, right);

    public static bool operator !=(TypeModel? left, TypeModel? right) => !Equals(left, right);
    #endregion

    public abstract TypeModel WithSymbol(Symbol symbol);

    public abstract bool IsSubtypeOf(TypeModel other);

    /// <summary>
    /// Convert to an outer type that is not optional. (Does not remove optional types inside collections.)
    /// </summary>
    public abstract NonOptionalType ToNonOptional();

    public abstract override string ToString();
}
