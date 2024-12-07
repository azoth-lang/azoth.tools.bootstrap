using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(EmptyTypeSymbol),
    typeof(BuiltInTypeSymbol),
    typeof(OrdinaryTypeSymbol),
    typeof(GenericParameterTypeSymbol),
    typeof(AssociatedTypeSymbol))]
public abstract class TypeSymbol : Symbol
{
    public override TypeName Name { get; }

    public bool IsGlobal => ContainingSymbol == Package;

    protected TypeSymbol(TypeName name)
    {
        Name = name;
    }

    public virtual DeclaredType? TryGetTypeConstructor() => null;

    public virtual IType? TryGetType() => null;
}
