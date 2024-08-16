using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

[Closed(
    typeof(ListType),
    typeof(SetType))]
public abstract class CollectionType : NonOptionalType
{
    public TypeModel ElementType { get; }

    private protected CollectionType(TypeModel elementType)
        : base(elementType.UnderlyingSymbol)
    {
        ElementType = elementType;
    }

    public abstract override CollectionType WithSymbol(Symbol symbol);
}
