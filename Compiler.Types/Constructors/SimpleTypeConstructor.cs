using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
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

    public sealed override TypeSemantics? Semantics => TypeSemantics.Value;

    public sealed override BuiltInTypeName Name { [DebuggerStepThrough] get; }

    public sealed override IFixedList<TypeConstructorParameter> Parameters => [];

    public sealed override bool AllowsVariance => false;

    public override bool HasIndependentParameters => false;

    public override IFixedList<GenericParameterTypeFactory> ParameterTypeFactories => [];

    public sealed override IFixedSet<BareType> Supertypes => BareType.AnySet;

    public sealed override ConstructedPlainType PlainType { [DebuggerStepThrough] get; }

    private protected SimpleTypeConstructor(BuiltInTypeName name)
    {
        Name = name;
        PlainType = new(this, containingType: null, []);
    }

    public sealed override ConstructedPlainType Construct(
        ConstructedPlainType? containingType,
        IFixedList<PlainType> arguments)
    {
        TypeRequires.NoArgs(arguments, nameof(arguments));
        return PlainType;
    }

    public sealed override PlainType TryConstructNullaryPlainType(ConstructedPlainType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), "Simple types do not have a containing type.");
        return PlainType;
    }

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
