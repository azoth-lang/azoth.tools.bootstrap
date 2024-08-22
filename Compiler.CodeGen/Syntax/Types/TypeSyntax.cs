using System;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

[Closed(
    typeof(SymbolTypeSyntax),
    typeof(CollectionTypeSyntax),
    typeof(OptionalTypeSyntax))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class TypeSyntax : IEquatable<TypeSyntax>
{
    public SymbolSyntax UnderlyingSymbol { get; }

    protected TypeSyntax(SymbolSyntax underlyingSymbol)
    {
        UnderlyingSymbol = underlyingSymbol;
    }

    #region Equality
    public abstract bool Equals(TypeSyntax? other);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TypeSyntax)obj);
    }

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
