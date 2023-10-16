using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(NamedBindingSymbol),
    typeof(SelfParameterSymbol))]
public abstract class BindingSymbol : Symbol
{
    public override PackageSymbol Package { get; }
    public override Symbol ContainingSymbol { get; }
    public bool IsMutableBinding { get; }
    public bool IsLentBinding { get; }
    public override SimpleName? Name { get; }

    public DataType DataType { get; }

    protected BindingSymbol(
        Symbol containingSymbol,
        bool isMutableBinding,
        bool isLentBinding,
        SimpleName? name,
        DataType dataType)
        : base(name)
    {
        Package = containingSymbol.Package ?? throw new ArgumentNullException(nameof(containingSymbol));
        ContainingSymbol = containingSymbol;
        Name = name;
        IsMutableBinding = isMutableBinding;
        DataType = dataType;
        IsLentBinding = isLentBinding;
    }
}
