using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class AnyTypeConstructor : TypeConstructor
{
    #region Singleton
    internal static readonly AnyTypeConstructor Instance = new();

    private AnyTypeConstructor() { }
    #endregion

    internal static readonly ConstructedPlainType PlainType = new(Instance, []);
    internal static readonly IFixedSet<ConstructedPlainType> Set = PlainType.Yield().ToFixedSet();

    TypeConstructorContext TypeConstructor.Context => PrimitiveContext.Instance;

    /// <summary>
    /// The `Any` type is like a trait in the sense that it is known to not have fields.
    /// </summary>
    public bool CanHaveFields => false;

    /// <summary>
    /// The `Any` type cannot be instantiated because it is abstract.
    /// </summary>
    public bool CanBeInstantiated => false;

    public TypeSemantics Semantics => TypeSemantics.Reference;
    public SpecialTypeName Name => SpecialTypeName.Any;
    TypeName TypeConstructor.Name => Name;

    IFixedList<TypeConstructorParameter> TypeConstructor.Parameters
        => FixedList.Empty<TypeConstructorParameter>();
    public bool AllowsVariance => false;

    IFixedList<GenericParameterPlainType> TypeConstructor.ParameterPlainTypes
        => FixedList.Empty<GenericParameterPlainType>();

    IFixedSet<ConstructedPlainType> TypeConstructor.Supertypes => [];

    #region Equality
    public bool Equals(TypeConstructor? other)
        // AnyTypeConstructor is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(AnyTypeConstructor));
    #endregion

    public IPlainType Construct(IFixedList<IPlainType> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Incorrect number of type arguments.");
        return PlainType;
    }

    public IMaybePlainType Construct(IFixedList<IMaybePlainType> typeArguments)
    {
        var properTypeArguments = typeArguments.ToFixedList().As<IPlainType>();
        if (properTypeArguments is null) return IPlainType.Unknown;
        return Construct((IFixedList<IMaybePlainType>)properTypeArguments.AsEnumerable());
    }

    public IPlainType TryConstructNullary() => PlainType;

    public override string ToString() => Name.ToString();

    public void ToString(StringBuilder builder) => builder.Append(Name);
}
