using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// Base class for type constructors for the types of literal values (e.g. <c>int[V]</c>,
/// <c>bool[V]</c>, etc.).
/// </summary>
[Closed(
       typeof(BoolLiteralTypeConstructor),
       typeof(IntegerLiteralTypeConstructor))]
// TODO possibly remove this type and make the literal types simple types
public abstract class LiteralTypeConstructor : SimpleOrLiteralTypeConstructor
{
    /// <remarks>Literal types don't carry state, so they don't have fields.</remarks>
    public override bool CanHaveFields => false;

    public sealed override bool CanBeInstantiated => true;

    public sealed override TypeSemantics Semantics => TypeSemantics.Value;

    public sealed override SpecialTypeName Name { get; }

    // TODO these need type parameters that are values
    public sealed override IFixedList<Parameter> Parameters => [];

    public sealed override bool AllowsVariance => false;

    public override bool HasIndependentParameters => false;

    public sealed override IFixedList<GenericParameterPlainType> ParameterPlainTypes => [];

    // TODO should this instead include the non-literal type (e.g. `int` or `bool`)?
    public sealed override IFixedSet<ConstructedBareType> Supertypes => BareType.AnySet;

    private protected LiteralTypeConstructor(SpecialTypeName name)
    {
        Name = name;
    }

    public abstract override SimpleTypeConstructor TryToNonLiteral();

    public sealed override ConstructedPlainType Construct(IFixedList<PlainType> arguments)
       => throw new NotImplementedException("Constructing literal types requires value type parameters.");

    /// <remarks>All literal types take a type parameter and cannot be nullary constructed.</remarks>
    public sealed override PlainType? TryConstructNullaryPlainType() => null;

    public sealed override void ToString(StringBuilder builder) => builder.Append(ToString());
}
