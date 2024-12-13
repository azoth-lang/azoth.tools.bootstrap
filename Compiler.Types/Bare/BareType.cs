using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// A bare type is a type that does not have a capability but could accept one.
/// </summary>
/// <remarks>While bare types do not have a capability prefix, they do have capabilities on their
/// type arguments. That is how they are distinct from plain types.</remarks>
[Closed(
    typeof(ConstructedBareType),
    typeof(AssociatedBareType))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class BareType : IEquatable<BareType>
{
    // Note: must use AnyTypeConstructor.PlainType instead of IPlainType.Any to avoid circular
    // dependency when initializing statics.
    public static readonly ConstructedBareType Any = new(AnyTypeConstructor.PlainType, []);
    public static readonly IFixedSet<ConstructedBareType> AnySet = Any.Yield().ToFixedSet();

    // TODO add containing type for capabilities on containing type arguments

    public abstract ConstructedOrAssociatedPlainType PlainType { get; }

    public abstract TypeConstructor? TypeConstructor { get; }

    public abstract IFixedList<IType> Arguments { get; }

    public abstract bool HasIndependentTypeArguments { get; }

    public abstract IFixedList<TypeParameterArgument> TypeParameterArguments { get; }

    public abstract TypeReplacements TypeReplacements { get; }

    public bool IsDeclaredConst => TypeConstructor?.IsDeclaredConst ?? false;

    public CapabilityType With(Capability capability)
        => CapabilityType.Create(capability, this);

    /// <summary>
    /// This type with whatever the default read capability is for the type based on whether it is
    /// declared `const`.
    /// </summary>
    public CapabilityType WithDefaultRead()
        => With(IsDeclaredConst ? Capability.Constant : Capability.Read);

    /// <summary>
    /// This type with whatever the default mutable capability is for the type based on whether it is
    /// declared `const`.
    /// </summary>
    public CapabilityType WithDefaultMutate()
        => With(IsDeclaredConst ? Capability.Constant : Capability.Mutable);

    public abstract BareType WithReplacement(IFixedList<IType> arguments);

    public virtual ConstructedBareType? TryToNonLiteral() => null;

    #region Equality
    public abstract bool Equals(BareType? other);

    public sealed override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is BareType other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public sealed override string ToString() => ToILString();

    public abstract string ToSourceCodeString();

    public abstract string ToILString();
}
