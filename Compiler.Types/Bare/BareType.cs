using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

[Closed(
    typeof(ConstructedBareType),
    typeof(AssociatedPlainType))]
// TODO rename to IBareType
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Using as a trait.")]
// ReSharper disable once InconsistentNaming
public interface BareType : IEquatable<BareType>
{
    // Note: must use AnyTypeConstructor.PlainType instead of IPlainType.Any to avoid circular
    // dependency when initializing statics.
    public static readonly ConstructedBareType Any = new(AnyTypeConstructor.PlainType, []);
    public static readonly IFixedSet<ConstructedBareType> AnySet = Any.Yield().ToFixedSet();

    ConstructedOrAssociatedPlainType PlainType { get; }

    TypeConstructor? TypeConstructor { get; }

    IFixedList<TypeParameterArgument> TypeParameterArguments { get; }

    CapabilityType With(Capability capability);

    CapabilityType WithRead();

    CapabilityType WithMutate();

    string ToSourceCodeString();

    string ToILString();
}
