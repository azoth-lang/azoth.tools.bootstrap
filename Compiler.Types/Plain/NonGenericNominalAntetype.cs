using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// An antetype that is not generic.
/// </summary>
/// <remarks>Non-generic antetypes are both an antetype and their own declared antetype.</remarks>
[Closed(
    typeof(EmptyAntetype),
    typeof(AnyAntetype),
    typeof(GenericParameterPlainType),
    typeof(SelfAntetype))]
public abstract class NonGenericNominalAntetype : NamedPlainType, ITypeConstructor
{
    public sealed override ITypeConstructor TypeConstructor => this;
    public TypeSemantics Semantics => TypeConstructor.Semantics;
    public abstract bool CanBeInstantiated { get; }
    public sealed override bool AllowsVariance => false;
    IFixedList<TypeConstructorParameter> ITypeConstructor.Parameters => [];
    IFixedList<GenericParameterPlainType> ITypeConstructor.GenericParameterPlainTypes => [];

    public virtual IAntetype Construct(IEnumerable<IAntetype> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Non-generic type cannot have type arguments", nameof(typeArguments));
        return this;
    }

    public IAntetype TryConstructNullary() => this;

    public override IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        => antetype;

    public bool Equals(ITypeConstructor? other)
        => other is IMaybeExpressionAntetype that && Equals(that);
}
