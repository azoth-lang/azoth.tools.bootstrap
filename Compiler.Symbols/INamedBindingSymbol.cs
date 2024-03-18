using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(typeof(INamedVariableSymbol), typeof(IFieldSymbol))]
public interface INamedBindingSymbol : IBindingSymbol
{
    public new IdentifierName Name { get; }

    public new DataType Type { get; }
}
