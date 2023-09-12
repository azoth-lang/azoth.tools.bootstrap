using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a variable or parameter. Both of which are bindings using `let` or `var`.
/// </summary>
public sealed class VariableSymbol : NamedBindingSymbol
{
    public override InvocableSymbol ContainingSymbol { get; }
    public int? DeclarationNumber { get; }
    public bool IsParameter { get; }
    public bool IsLocal => !IsParameter;

    public VariableSymbol(
        InvocableSymbol containingSymbol,
        Name name,
        int? declarationNumber,
        bool isMutableBinding,
        DataType dataType,
        bool isParameter)
        : base(containingSymbol, name, isMutableBinding, dataType)
    {
        ContainingSymbol = containingSymbol;
        DeclarationNumber = declarationNumber;
        IsParameter = isParameter;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is VariableSymbol otherVariable
               && ContainingSymbol == otherVariable.ContainingSymbol
               && Name == otherVariable.Name
               && DeclarationNumber == otherVariable.DeclarationNumber
               && IsMutableBinding == otherVariable.IsMutableBinding
               && DataType == otherVariable.DataType;
    }

    public override int GetHashCode()
        => HashCode.Combine(Name, DeclarationNumber, IsMutableBinding, DataType);

    public override string ToILString()
    {
        var mutable = IsMutableBinding ? "var" : "let";
        var declarationNumber = DeclarationNumber is null ? "" : "#" + DeclarationNumber;
        return $"{ContainingSymbol.ToILString()} {{{mutable} {Name}{declarationNumber}: {DataType.ToILString()}}}";
    }
}
