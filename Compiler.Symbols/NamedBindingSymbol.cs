using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(VariableSymbol),
    typeof(FieldSymbol))]
public abstract class NamedBindingSymbol : BindingSymbol
{
    public override Name Name { get; }

    protected NamedBindingSymbol(
        Symbol containingSymbol,
        Name name,
        bool isMutableBinding,
        DataType dataType)
        : base(containingSymbol, name, isMutableBinding, dataType)
    {
        Name = name;
    }
}
