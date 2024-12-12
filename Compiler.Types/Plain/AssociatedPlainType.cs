using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OrdinaryAssociatedPlainType),
    typeof(SelfPlainType))]
public abstract class AssociatedPlainType : ConstructedOrAssociatedPlainType
{
    public TypeConstructor ContainingType { get; }
    public sealed override TypeConstructor? TypeConstructor => null;
    public sealed override TypeSemantics? Semantics => null;
    public sealed override bool AllowsVariance => false;
    internal sealed override PlainTypeReplacements TypeReplacements => PlainTypeReplacements.None;

    protected AssociatedPlainType(TypeConstructor containingType)
    {
        ContainingType = containingType;
    }

    public sealed override string ToString() => $"{ContainingType}.{Name}";

    public sealed override string ToBareString() => ToString();
}
