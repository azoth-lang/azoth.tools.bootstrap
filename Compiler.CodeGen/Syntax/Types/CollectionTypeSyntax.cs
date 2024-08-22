using System;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

public sealed class CollectionTypeSyntax : TypeSyntax
{
    public CollectionKind Kind { get; }
    public TypeSyntax Referent { get; }

    public CollectionTypeSyntax(CollectionKind kind, TypeSyntax referent)
        : base(referent.UnderlyingSymbol)
    {
        Kind = kind;
        Referent = referent;
    }

    #region Equality
    public override bool Equals(TypeSyntax? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CollectionTypeSyntax type
               && Referent.Equals(type.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(Kind, Referent);
    #endregion

    public override string ToString()
        => Kind switch
        {
            CollectionKind.List => $"{Referent}*",
            CollectionKind.Set => $"{{{Referent}}}",
            CollectionKind.Enumerable => $"IEnumerable<{Referent}>",
            _ => throw ExhaustiveMatch.Failed(Kind)
        };
}
