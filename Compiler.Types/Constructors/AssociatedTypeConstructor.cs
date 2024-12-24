using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(typeof(OrdinaryAssociatedTypeConstructor), typeof(SelfTypeConstructor))]
public abstract class AssociatedTypeConstructor : BareTypeConstructor
{
    public sealed override BareTypeConstructor Context { [DebuggerStepThrough] get; }
    public sealed override bool CanBeInstantiated => false;
    /// <remarks>While the associated type itself won't have fields, the type it is instantiated to
    /// might. However, it probably doesn't matter since I believe this is used for supertypes and
    /// associated types can't be supertypes.</remarks>
    public sealed override bool CanHaveFields => true;
    public sealed override bool CanBeSupertype => false;
    public sealed override IFixedList<TypeConstructorParameter> Parameters => [];
    public sealed override bool AllowsVariance => false;
    public sealed override bool HasIndependentParameters => false;
    public sealed override IFixedList<GenericParameterTypeFactory> ParameterTypeFactories => [];

    protected AssociatedTypeConstructor(BareTypeConstructor context)
    {
        Context = context;
    }

    /// <remarks>Unlike other type constructors, associated types will default to a containing type
    /// that is constructed with the parameter plain types. This is because when referenced from
    /// within a type definition they are referenced without a containing type and the containing
    /// type is implied.</remarks>
    public override ConstructedPlainType Construct(
        ConstructedPlainType? containingType,
        IFixedList<PlainType> arguments)
    {
        TypeRequires.NoArgs(arguments, nameof(arguments));
        return new(this, containingType ?? Context.ConstructWithParameterPlainTypes(), arguments);
    }

    /// <remarks>Unlike other type constructors, associated types will default to a containing type
    /// that is constructed with the parameter types. This is because when referenced from
    /// within a type definition they are referenced without a containing type and the containing
    /// type is implied.</remarks>
    public override BareType Construct(BareType? containingType, IFixedList<Type> arguments)
        => base.Construct(containingType
                          // TODO is it right to hide the requirement to match the plain type?
                          ?? Context.ConstructWithParameterTypes(), arguments);

    /// <remarks>Unlike other type constructors, associated types will default to a containing type
    /// that is constructed with the parameter plain types. This is because when referenced from
    /// within a type definition they are referenced without a containing type and the containing
    /// type is implied.</remarks>
    public sealed override ConstructedPlainType TryConstructNullaryPlainType(ConstructedPlainType? containingType)
        => new(this, containingType ?? Context.ConstructWithParameterPlainTypes(), []);

    #region Equality
    public sealed override bool Equals(BareTypeConstructor? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is AssociatedTypeConstructor otherTypeConstructor
            && Context.Equals(otherTypeConstructor.Context)
            && Name.Equals(otherTypeConstructor.Name);
    }

    public sealed override int GetHashCode() => HashCode.Combine(Context, Name);
    #endregion

    public sealed override void ToString(StringBuilder builder)
    {
        builder.Append(Context);
        builder.Append('.');
        builder.Append(Name);
    }
}
