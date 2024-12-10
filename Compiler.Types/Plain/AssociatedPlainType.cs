using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
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

    public CapabilityType With(Capability capability)
        => new CapabilityType(capability, this);

    public CapabilityType WithRead() => With(Capability.Read);

    public CapabilityType WithMutate() => With(Capability.Mutable);

    public sealed override string ToString() => $"{ContainingType}.{Name}";
}
