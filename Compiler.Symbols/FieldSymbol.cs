using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public sealed class FieldSymbol : NamedBindingSymbol
{
    public override TypeSymbol ContainingSymbol { get; }

    public FieldSymbol(
        TypeSymbol containingSymbol,
        Name name,
        bool isMutableBinding,
        DataType dataType)
        : base(containingSymbol, isMutableBinding, false, name, dataType)
    {
        ContainingSymbol = containingSymbol;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FieldSymbol otherField
               && ContainingSymbol == otherField.ContainingSymbol
               && Name == otherField.Name
               && IsMutableBinding == otherField.IsMutableBinding
               && DataType == otherField.DataType;
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingSymbol, Name, IsMutableBinding, DataType);

    public override string ToILString()
    {
        var mutable = IsMutableBinding ? "var" : "let";
        return $"{ContainingSymbol.ToILString()}::{mutable} {Name}: {DataType.ToILString()}";
    }
}
