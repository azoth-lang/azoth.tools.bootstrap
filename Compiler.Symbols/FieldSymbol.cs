using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class FieldSymbol : Symbol
{
    public override PackageSymbol Package => ContainingSymbol.Package;
    public override OrdinaryTypeSymbol ContainingSymbol { get; }
    public override OrdinaryTypeSymbol ContextTypeSymbol => ContainingSymbol;
    public override IdentifierName Name { get; }
    public NonVoidType Type { get; }
    public bool IsMutableBinding { get; }

    public FieldSymbol(
        OrdinaryTypeSymbol containingSymbol,
        bool isMutableBinding,
        IdentifierName name,
        NonVoidType type)
    {
        ContainingSymbol = containingSymbol;
        IsMutableBinding = isMutableBinding;
        Name = name;
        Type = type;
    }

    #region Equality
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FieldSymbol otherField
               && ContainingSymbol == otherField.ContainingSymbol
               && IsMutableBinding == otherField.IsMutableBinding
               && Name == otherField.Name
               && Type.Equals(otherField.Type);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, IsMutableBinding, Name, Type);
    #endregion

    public override string ToILString()
    {
        var mutable = IsMutableBinding ? "var" : "let";
        return $"{ContainingSymbol.ToILString()}::{mutable} {Name}: {Type.ToILString()}";
    }
}
