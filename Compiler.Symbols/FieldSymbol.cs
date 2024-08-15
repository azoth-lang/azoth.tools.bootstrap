using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class FieldSymbol : BindingSymbol, IFieldSymbol
{
    public override IdentifierName Name { get; }
    public override DataType Type { get; }
    public override UserTypeSymbol ContainingSymbol { get; }
    public override UserTypeSymbol ContextTypeSymbol => ContainingSymbol;

    public FieldSymbol(
        UserTypeSymbol containingSymbol,
        IdentifierName name,
        bool isMutableBinding,
        DataType dataType)
        : base(containingSymbol, isMutableBinding, false, name)
    {
        Name = name;
        Type = dataType;
        ContainingSymbol = containingSymbol;
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
               && Type == otherField.Type;
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
