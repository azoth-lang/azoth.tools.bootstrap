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
        bool isMutableBinding,
        bool isLentBinding,
        Name name,
        DataType dataType)
        : base(containingSymbol, isMutableBinding, isLentBinding, name, dataType)
    {
        Name = name;
    }
}
