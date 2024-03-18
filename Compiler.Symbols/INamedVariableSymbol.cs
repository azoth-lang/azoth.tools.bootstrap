namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public interface INamedVariableSymbol : IBindingSymbol, IVariableSymbol, INamedBindingSymbol
{
    public int? DeclarationNumber { get; }
    public bool IsParameter { get; }
    public bool IsLocal => !IsParameter;
}
