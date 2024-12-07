using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class FieldSymbol : Symbol
{
    public override IdentifierName Name { get; }
    public INonVoidType Type { get; }
    public override OrdinaryTypeSymbol ContainingSymbol { get; }
    public override OrdinaryTypeSymbol ContextTypeSymbol => ContainingSymbol;
    public override PackageSymbol Package { get; }
    public bool IsMutableBinding { get; }

    public FieldSymbol(
        OrdinaryTypeSymbol containingSymbol,
        IdentifierName name,
        bool isMutableBinding,
        INonVoidType dataType)
    {
        Name = name;
        Type = dataType;
        Package = containingSymbol.Package ?? throw new ArgumentNullException(nameof(containingSymbol));
        ContainingSymbol = containingSymbol;
        IsMutableBinding = isMutableBinding;
    }

    #region Equality
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FieldSymbol otherField
               && ContainingSymbol == otherField.ContainingSymbol
               && Name == otherField.Name
               && IsMutableBinding == otherField.IsMutableBinding
               && Type.Equals(otherField.Type);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, IsMutableBinding, Type);
    #endregion

    public override string ToILString()
    {
        var mutable = IsMutableBinding ? "var" : "let";
        return $"{ContainingSymbol.ToILString()}::{mutable} {Name}: {Type.ToILString()}";
    }
}
