using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;
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
        return syntax switch
        {
            SymbolTypeSyntax syn => SymbolType.CreateFromSyntax(tree, syn.Symbol),
            CollectionTypeSyntax syn => CreateFromSyntax(tree, syn),
            OptionalTypeSyntax syn => new OptionalType(CreateFromSyntax(tree, syn.Referent)),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
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

    public abstract TypeModel WithSymbol(Symbol symbol);

    public abstract bool IsSubtypeOf(TypeModel other);

    /// <summary>
    /// Convert to an outer type that is not optional. (Does not remove optional types inside collections.)
    /// </summary>
    public abstract NonOptionalType ToNonOptional();

    public TreeNodeModel? ReferencedNode() => (UnderlyingSymbol as InternalSymbol)?.ReferencedNode;

    public abstract override string ToString();
}
