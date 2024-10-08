using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(EmptyTypeSymbol),
    typeof(PrimitiveTypeSymbol),
    typeof(UserTypeSymbol),
    typeof(GenericParameterTypeSymbol))]
public abstract class TypeSymbol : Symbol
{
    public override TypeName Name { get; }

    public bool IsGlobal => ContainingSymbol == Package;

    protected TypeSymbol(TypeName name)
    {
        Name = name;
    }

    public virtual DeclaredType? GetDeclaredType() => null;

    public virtual IType? GetDataType() => null;
}
