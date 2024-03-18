namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public interface IFieldSymbol : IBindingSymbol, INamedBindingSymbol
{
    public new UserTypeSymbol ContainingSymbol { get; }
}
