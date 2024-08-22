using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

[Closed(
    typeof(ListTypeModel),
    typeof(SetTypeModel),
    typeof(EnumerableTypeModel))]
public abstract class CollectionTypeModel : NonOptionalType
{
    public TypeModel ElementType { get; }

    private protected CollectionTypeModel(TypeModel elementType)
        : base(elementType.UnderlyingSymbol)
    {
        ElementType = elementType;
    }

    public abstract override CollectionTypeModel WithSymbol(Symbol symbol);
}
