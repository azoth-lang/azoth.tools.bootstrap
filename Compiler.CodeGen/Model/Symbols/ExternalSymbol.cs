using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
public sealed class ExternalSymbol : Symbol
{
    public override string FullName { get; }

    public ExternalSymbol(string fullName)
    {
        FullName = fullName;
        Requires.NotNullOrEmpty(nameof(fullName), fullName);
    }

    #region Equality
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ExternalSymbol symbol
               && FullName == symbol.FullName;
    }

    public override int GetHashCode() => HashCode.Combine(FullName);
    #endregion

    public override int GetEquivalenceHashCode() => HashCode.Combine(FullName);

    public override string ToString() => $"`{FullName}`";
}
