using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(NamedVariableSymbol),
    typeof(FieldSymbol))]
public abstract class NamedBindingSymbol : BindingSymbol, INamedBindingSymbol
{
    public override IdentifierName Name { get; }

    public override DataType Type { get; }

    protected NamedBindingSymbol(
        Symbol containingSymbol,
        bool isMutableBinding,
        bool isLentBinding,
        IdentifierName name,
        DataType dataType)
        : base(containingSymbol, isMutableBinding, isLentBinding, name)
    {
        Name = name;
        Type = dataType;
    }
}
