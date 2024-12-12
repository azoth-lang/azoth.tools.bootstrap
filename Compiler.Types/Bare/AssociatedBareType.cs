using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// A bare type that is an associated type.
/// </summary>
/// <remarks>While this is just a thin wrapper around <see cref="AssociatedPlainType"/>, having it
/// rather than making <see cref="AssociatedPlainType"/> directly be a <see cref="BareType"/>
/// enables a number of useful things. <see cref="BareType"/> can be a class.
/// <see cref="AssociatedPlainType"/> isn't contaminated with <see cref="BareType"/> related
/// functionality (i.e. the type hierarchy is clean). <see cref="ConstructedBareType"/> is kept so
/// that supertypes can be represented by it without code having to deal with associated types in
/// supertype position (which isn't allowed, but would be representable if the bare type hierarchy
/// was merged into <see cref="BareType"/>.</remarks>
public sealed class AssociatedBareType : BareType
{
    public override AssociatedPlainType PlainType { get; }
    public override TypeConstructor? TypeConstructor => null;
    public override IFixedList<IType> Arguments => [];
    public override IFixedList<TypeParameterArgument> TypeParameterArguments => [];

    public AssociatedBareType(AssociatedPlainType plainType)
    {
        PlainType = plainType;
    }

    #region Equality
    public override bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is AssociatedBareType otherType
               && PlainType.Equals(otherType.PlainType);
    }

    public override int GetHashCode() => HashCode.Combine(PlainType);
    #endregion

    public override string ToSourceCodeString() => PlainType.ToString();

    public override string ToILString() => PlainType.ToString();
}
