using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(PrimitiveTypeSymbol),
    typeof(ObjectTypeSymbol))]
public abstract class TypeSymbol : Symbol
{
    public new NamespaceOrPackageSymbol? ContainingSymbol { get; }
    public new TypeName Name { get; }
    public DataType DeclaresDataType { get; }

    protected TypeSymbol(NamespaceOrPackageSymbol? containingSymbol, TypeName name, DataType declaresDataType)
        : base(containingSymbol, name)
    {
        ContainingSymbol = containingSymbol;
        Name = name;
        DeclaresDataType = declaresDataType;
    }
}
