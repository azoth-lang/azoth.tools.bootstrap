using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(typeof(FieldSymbol))]
public abstract class BindingSymbol : Symbol, IBindingSymbol
{
    public override PackageSymbol Package { get; }
    public override Symbol ContainingSymbol { get; }
    public bool IsMutableBinding { get; }
    public override IdentifierName? Name { get; }
    public abstract Pseudotype Type { get; }

    protected BindingSymbol(
        Symbol containingSymbol,
        bool isMutableBinding,
        IdentifierName? name)
    {
        Package = containingSymbol.Package ?? throw new ArgumentNullException(nameof(containingSymbol));
        ContainingSymbol = containingSymbol;
        Name = name;
        IsMutableBinding = isMutableBinding;
    }
}
