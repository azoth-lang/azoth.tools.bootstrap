using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[Closed(typeof(NonOptionalTypeModel), typeof(OptionalTypeModel))]
public abstract class TypeModel : IEquatable<TypeModel>
{
    [return: NotNullIfNotNull(nameof(syntax))]
    public static TypeModel? CreateFromSyntax(TreeModel tree, TypeSyntax? syntax)
    {
        if (syntax is null)
            return null;
        return syntax switch
        {
            SymbolTypeSyntax syn => SymbolTypeModel.CreateFromSyntax(tree, syn.Symbol),
            CollectionTypeSyntax syn => CreateFromSyntax(tree, syn),
            OptionalTypeSyntax syn => CreateFromSyntax(tree, syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
    }

    private static OptionalTypeModel CreateFromSyntax(TreeModel tree, OptionalTypeSyntax syntax)
    {
        if (CreateFromSyntax(tree, syntax.Referent) is not NonOptionalTypeModel nonOptionalType)
            throw new InvalidOperationException("Optional type must have a non-optional referent.");
        return new(nonOptionalType);
    }

    public static TypeModel CreateFromSyntax(TreeModel tree, CollectionTypeSyntax syntax)
        => syntax.Kind switch
        {
            CollectionKind.List => new ListTypeModel(CreateFromSyntax(tree, syntax.Referent)),
            CollectionKind.Set => new SetTypeModel(CreateFromSyntax(tree, syntax.Referent)),
            CollectionKind.Enumerable => new EnumerableTypeModel(CreateFromSyntax(tree, syntax.Referent)),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public Symbol UnderlyingSymbol { get; }
    public abstract bool IsValueType { get; }

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

    /// <summary>
    /// This type with the <see cref="UnderlyingSymbol"/> replaced with the given
    /// <paramref name="symbol"/> as an optional type.
    /// </summary>
    public abstract TypeModel WithOptionalSymbol(Symbol symbol);

    public abstract bool IsSubtypeOf(TypeModel other);

    /// <summary>
    /// Remove the top-level optional type if it exists
    /// </summary>
    /// <remarks>Does not affect nested optional types.</remarks>
    public abstract NonOptionalTypeModel ToNonOptional();

    /// <summary>
    /// Make this type optional if it isn't already.
    /// </summary>
    public abstract OptionalTypeModel ToOptional();

    public TreeNodeModel? ReferencedNode() => (UnderlyingSymbol as InternalSymbol)?.ReferencedNode;

    public abstract override string ToString();
}
