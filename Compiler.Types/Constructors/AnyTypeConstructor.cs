using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public sealed class AnyTypeConstructor : BareTypeConstructor
{
    #region Singleton
    internal static readonly AnyTypeConstructor Instance = new();

    private AnyTypeConstructor() { }
    #endregion

    internal static readonly BarePlainType PlainType = new(Instance, containingType: null, []);

    public override BuiltInContext Context => BuiltInContext.Instance;

    public override bool IsDeclaredConst => false;

    /// <summary>
    /// The `Any` type is like a trait in the sense that it is known to not have fields.
    /// </summary>
    public override bool CanHaveFields => false;

    public override bool CanBeBaseType => false;
    public override bool CanBeSupertype => false;

    /// <summary>
    /// The `Any` type cannot be instantiated because it is abstract.
    /// </summary>
    public override bool CanBeInstantiated => false;

    public override TypeSemantics? Semantics => TypeSemantics.Reference;
    public override BuiltInTypeName Name => BuiltInTypeName.Any;

    public override IFixedList<TypeConstructorParameter> Parameters => [];
    public override bool AllowsVariance => false;
    public override bool HasIndependentParameters => false;
    public override IFixedList<GenericParameterTypeConstructor> ParameterTypeFactories => [];

    public override BareType? BaseType => null;

    /// <remarks>Because `Any` is the base of the constructed type hierarchy, it has no supertypes.</remarks>
    public override IFixedSet<BareType> Supertypes => [];

    public override BarePlainType Construct(
        BarePlainType? containingType,
        IFixedList<PlainType> arguments)
    {
        Requires.Null(containingType, nameof(containingType), "Any does not have a containing type.");
        TypeRequires.NoArgs(arguments, nameof(arguments));
        return PlainType;
    }

    public override PlainType TryConstructNullaryPlainType(BarePlainType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), "Any does not have a containing type.");
        return PlainType;
    }

    #region Equality
    public override bool Equals(BareTypeConstructor? other)
        // AnyTypeConstructor is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(AnyTypeConstructor));
    #endregion

    public override string ToString() => Name.ToString();

    public override void ToString(StringBuilder builder) => builder.Append(Name);
}
