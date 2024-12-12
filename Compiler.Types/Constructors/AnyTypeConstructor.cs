using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public sealed class AnyTypeConstructor : TypeConstructor
{
    #region Singleton
    internal static readonly AnyTypeConstructor Instance = new();

    private AnyTypeConstructor() { }
    #endregion

    internal static readonly ConstructedPlainType PlainType = new(Instance, []);

    public override PrimitiveContext Context => PrimitiveContext.Instance;

    public override bool IsDeclaredConst => false;

    /// <summary>
    /// The `Any` type is like a trait in the sense that it is known to not have fields.
    /// </summary>
    public override bool CanHaveFields => false;

    public override bool CanBeSupertype => true;

    /// <summary>
    /// The `Any` type cannot be instantiated because it is abstract.
    /// </summary>
    public override bool CanBeInstantiated => false;

    public override TypeSemantics Semantics => TypeSemantics.Reference;
    public override SpecialTypeName Name => SpecialTypeName.Any;

    public override IFixedList<Parameter> Parameters => [];
    public override bool AllowsVariance => false;
    public override bool HasIndependentParameters => false;
    public override IFixedList<GenericParameterPlainType> ParameterPlainTypes => [];

    /// <remarks>Because `Any` is the base of the constructed type hierarchy, it has no supertypes.</remarks>
    public override IFixedSet<ConstructedBareType> Supertypes => [];

    public override ConstructedPlainType Construct(IFixedList<IPlainType> arguments)
    {
        if (!arguments.IsEmpty) throw new ArgumentException("Incorrect number of type arguments.");
        return PlainType;
    }

    public override IPlainType TryConstructNullaryPlainType() => PlainType;

    #region Equality
    public override bool Equals(TypeConstructor? other)
        // AnyTypeConstructor is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(AnyTypeConstructor));
    #endregion

    public override string ToString() => Name.ToString();

    public override void ToString(StringBuilder builder) => builder.Append(Name);
}
