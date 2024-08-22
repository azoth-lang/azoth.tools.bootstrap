using System;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

public sealed class OptionalTypeSyntax : TypeSyntax
{
    public TypeSyntax Referent { get; }

    public OptionalTypeSyntax(TypeSyntax referent)
        : base(referent.UnderlyingSymbol)
    {
        Referent = referent;
    }

    #region Equality
    public override bool Equals(TypeSyntax? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OptionalTypeSyntax type && Referent.Equals(type.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(Referent, typeof(OptionalTypeSyntax));
    #endregion

    public override string ToString() => $"{Referent}?";
}
