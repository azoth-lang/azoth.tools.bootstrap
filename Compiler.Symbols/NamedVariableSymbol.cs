using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a variable or parameter. Both of which are bindings using `let` or `var`.
/// </summary>
public sealed class NamedVariableSymbol : NamedBindingSymbol, INamedVariableSymbol
{
    public static NamedVariableSymbol CreateLocal(
        InvocableSymbol containingSymbol,
        bool isMutableBinding,
        IdentifierName name,
        int? declarationNumber,
        DataType dataType)
        => new(containingSymbol, isMutableBinding, false, name, declarationNumber, dataType, false);

    public static NamedVariableSymbol CreateParameter(
        InvocableSymbol containingSymbol,
        IdentifierName name,
        int? declarationNumber,
        bool isMutableBinding,
        bool isLentBinding,
        DataType dataType)
        => new(containingSymbol, isMutableBinding, isLentBinding, name, declarationNumber, dataType, true);

    public override InvocableSymbol ContainingSymbol { get; }
    public override TypeSymbol? ContextTypeSymbol => null;
    public int? DeclarationNumber { get; }
    public bool IsParameter { get; }
    public bool IsLocal => !IsParameter;

    private NamedVariableSymbol(
        InvocableSymbol containingSymbol,
        bool isMutableBinding,
        bool isLentBinding,
        IdentifierName name,
        int? declarationNumber,
        DataType dataType,
        bool isParameter)
        : base(containingSymbol, isMutableBinding, isLentBinding, name, dataType)
    {
        ContainingSymbol = containingSymbol;
        DeclarationNumber = declarationNumber;
        IsParameter = isParameter;
    }

    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is NamedVariableSymbol otherVariable
               && ContainingSymbol == otherVariable.ContainingSymbol
               && Name == otherVariable.Name
               && DeclarationNumber == otherVariable.DeclarationNumber
               && IsMutableBinding == otherVariable.IsMutableBinding
               && Type == otherVariable.Type;
    }

    public override int GetHashCode()
        => HashCode.Combine(Name, DeclarationNumber, IsMutableBinding, Type);

    public override string ToILString()
    {
        var mutable = IsMutableBinding ? "var" : "let";
        var declarationNumber = DeclarationNumber is null ? "" : "#" + DeclarationNumber;
        return $"{mutable} {Name}{declarationNumber}: {Type.ToILString()} in {ContainingSymbol.ToILString()}";
    }
}
