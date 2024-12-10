using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OrdinaryAssociatedPlainType),
    typeof(SelfPlainType))]
public abstract class AssociatedPlainType : VariablePlainType, BareType
{
    public TypeConstructor ContainingType { get; }

    protected AssociatedPlainType(TypeConstructor containingType)
    {
        ContainingType = containingType;
    }

    public sealed override string ToString() => $"{ContainingType}.{Name}";
}
