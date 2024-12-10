using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

public sealed class ConstructedBareType : BareType
{
    public ConstructedPlainType PlainType { get; }
    public IFixedList<IType> TypeArguments { get; }

    public ConstructedBareType(ConstructedPlainType plainType, IFixedList<IType> typeArguments)
    {
        Requires.That(plainType.TypeArguments.SequenceEqual(typeArguments.Select(a => a.PlainType)),
            nameof(typeArguments), "Type arguments must match plain type.");
        PlainType = plainType;
        TypeArguments = typeArguments;
    }

    public CapabilityType With(Capability capability)
        => new CapabilityType(capability, PlainType, TypeArguments);

    public CapabilityType WithRead() => With(Capability.Read);

    public CapabilityType WithMutate() => With(Capability.Mutable);
}
