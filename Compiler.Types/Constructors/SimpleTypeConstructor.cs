using System.Runtime.CompilerServices;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(BoolTypeConstructor),
    typeof(IntegerTypeConstructor))]
public abstract class SimpleTypeConstructor : SimpleOrLiteralTypeConstructor
{
    /// <remarks>Simple types are all structs with values so they effectively do have fields.</remarks>
    public override bool CanHaveFields => true;

    public sealed override bool CanBeInstantiated => true;

    public sealed override TypeSemantics Semantics => TypeSemantics.Value;

    public sealed override SpecialTypeName Name { get; }

    public sealed override IFixedList<Parameter> Parameters => [];

    public sealed override bool AllowsVariance => false;

    public override bool HasIndependentParameters => false;

    public sealed override IFixedList<GenericParameterPlainType> ParameterPlainTypes => [];

    public sealed override IFixedSet<Supertype> Supertypes => Supertype.AnySet;

    public sealed override ConstructedPlainType PlainType { get; }

    private protected SimpleTypeConstructor(SpecialTypeName name)
    {
        Name = name;
        PlainType = new(this, []);
    }

    public sealed override ConstructedPlainType Construct(IFixedList<IPlainType> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Simple type cannot have type arguments", nameof(typeArguments));
        return PlainType;
    }

    public sealed override IPlainType TryConstructNullary() => PlainType;

    #region Equality
    public sealed override bool Equals(TypeConstructor? other)
        // All simple type constructors are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);



    public sealed override int GetHashCode()
        => RuntimeHelpers.GetHashCode(this);
    #endregion

    public sealed override string ToString() => Name.ToString();

    public sealed override void ToString(StringBuilder builder) => builder.Append(Name);
}
