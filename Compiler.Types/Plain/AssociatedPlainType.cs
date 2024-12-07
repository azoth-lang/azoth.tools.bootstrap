using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(SelfPlainType))]
public abstract class AssociatedPlainType : VariablePlainType
{
    public OrdinaryTypeConstructor ContainingType { get; }

    protected AssociatedPlainType(OrdinaryTypeConstructor containingType)
    {
        ContainingType = containingType;
    }

    public sealed override string ToString() => $"{ContainingType}.{Name}";
}
