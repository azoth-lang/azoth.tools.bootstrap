using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

[Closed(
    typeof(ListType),
    typeof(SetType))]
public abstract class CollectionType : Type
{
    public Type ElementType { get; }

    private protected CollectionType(Type elementType)
        : base(elementType.UnderlyingSymbol)
    {
        ElementType = elementType;
    }

    public abstract override CollectionType WithSymbol(Symbol symbol);
}
