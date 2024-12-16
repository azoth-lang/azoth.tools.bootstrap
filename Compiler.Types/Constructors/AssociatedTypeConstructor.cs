using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(typeof(OrdinaryAssociatedTypeConstructor), typeof(SelfTypeConstructor))]
public abstract class AssociatedTypeConstructor : TypeConstructor
{
    public sealed override TypeConstructor Context { get; }
    public sealed override bool IsDeclaredConst => false;
    public sealed override bool CanBeInstantiated => false;
    /// <remarks>While the associated type itself won't have fields, the type it is instantiated to
    /// might. However, it probably doesn't matter since I believe this is used for supertypes and
    /// associated types can't be supertypes.</remarks>
    public sealed override bool CanHaveFields => true;
    public sealed override bool CanBeSupertype => false;
    public sealed override TypeSemantics? Semantics => null;
    public sealed override IFixedList<Parameter> Parameters => [];
    public sealed override bool AllowsVariance => false;
    public sealed override bool HasIndependentParameters => false;
    public sealed override IFixedList<GenericParameterTypeFactory> ParameterTypeFactories => [];

    public ConstructedPlainType PlainType { get; }

    protected AssociatedTypeConstructor(TypeConstructor context)
    {
        Context = context;
        PlainType = new(this, []);
    }

    public override ConstructedPlainType Construct(IFixedList<PlainType> arguments)
    {
        TypeRequires.NoArgs(arguments, nameof(arguments));
        return PlainType;
    }

    public sealed override ConstructedPlainType TryConstructNullaryPlainType()
        => PlainType;

    #region Equality
    public sealed override bool Equals(TypeConstructor? other)
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
