using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

public sealed class VoidType : Type
{
    #region Singleton
    public static VoidType Instance { get; } = new();

    private VoidType() { }
    #endregion

    #region Equality
    public override bool Equals(Type? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is VoidType;
    }

    public override int GetHashCode() => HashCode.Combine(typeof(VoidType));

    #endregion

    public override int GetEquivalenceHashCode() => HashCode.Combine(typeof(VoidType));

    public override VoidType WithSymbol(Symbol symbol) => this;

    public override bool IsSubtypeOf(Type other) => other is VoidType;

    public override string ToString() => "void";
}
