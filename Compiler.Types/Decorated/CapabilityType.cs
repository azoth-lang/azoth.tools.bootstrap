using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// e.g. `mut Foo`, `const Self`, etc. when not applied to GenericParameterPlainType
public sealed class CapabilityType : NonVoidType
{
    public static CapabilityType Create(Capability capability, ConstructedPlainType plainType)
        => new(capability, new BareType(plainType, []));

    public static CapabilityType Create(Capability capability, BareType bareType)
        => new(capability, bareType);

    public Capability Capability { get; }

    public BareType BareType { get; }
    public override ConstructedPlainType PlainType => BareType.PlainType;

    public TypeConstructor? TypeConstructor => BareType.TypeConstructor;

    public IFixedList<Type> Arguments => BareType.Arguments;

    public override bool HasIndependentTypeArguments => BareType.HasIndependentTypeArguments;

    public IFixedList<TypeParameterArgument> TypeParameterArguments => BareType.TypeParameterArguments;

    public override TypeReplacements TypeReplacements => BareType.TypeReplacements;

    private CapabilityType(
        Capability capability,
        BareType bareType)
    {
        Capability = capability;
        BareType = bareType;
    }

    public override CapabilityType ToNonLiteral()
    {
        var newBareType = BareType.TryToNonLiteral();
        if (newBareType is null) return this;
        return new(Capability, newBareType);
    }

    public CapabilityType With(Capability capability)
    {
        // Avoid allocating a new CapabilityType when it isn't needed
        if (Capability.Equals(capability)) return this;
        return BareType.With(capability);
    }

    public CapabilityType With(IFixedList<Type> arguments)
    {
        // Avoid allocating a new CapabilityType when it isn't needed
        if (Arguments.Equals(arguments)) return this;
        return BareType.WithReplacement(arguments).With(Capability);
    }

    // TODO this method represents an invalid operation and should be eliminated (the parameters do
    // not provide enough information to properly determine what to upcast to).
    public CapabilityType UpcastTo(TypeConstructor target)
    {
        if (TypeConstructor?.Equals(target) ?? false) return this;

        // TODO this will fail if the type implements the target type in multiple ways.
        var supertype = TypeConstructor?.Supertypes.Where(s => s.TypeConstructor.Equals(target)).TrySingle();
        if (supertype is null) throw new ArgumentException($"The type {target} is not a supertype of {ToILString()}.");

        var bareType = new BareType(supertype.PlainType, supertype.Arguments);
        bareType = TypeReplacements.ReplaceTypeParametersIn(bareType);

        return bareType.With(Capability);
    }

    #region Equality
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilityType otherType
               && Capability.Equals(otherType.Capability)
               && BareType.Equals(otherType.BareType);
    }

    public override int GetHashCode() => HashCode.Combine(Capability, BareType);
    #endregion

    public override string ToSourceCodeString()
        => $"{Capability.ToSourceCodeString()} {BareType.ToSourceCodeString()}";

    public override string ToILString()
        => $"{Capability.ToILString()} {BareType.ToILString()}";
}
