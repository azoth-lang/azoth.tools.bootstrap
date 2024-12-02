using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// An antetype that is not generic.
/// </summary>
/// <remarks>Non-generic antetypes are both an antetype and their own declared antetype.</remarks>
[Closed(
    typeof(EmptyPlainType),
    typeof(GenericParameterPlainType),
    typeof(SelfPlainType))]
// TODO maybe this class should be eliminated all together
public abstract class NonGenericNominalAntetype : NamedPlainType
{
    public sealed override ITypeConstructor? TypeConstructor => null;
    public TypeSemantics? Semantics => null;
    public abstract bool CanBeInstantiated { get; }
    public sealed override bool AllowsVariance => false;

    public override IMaybeAntetype ReplaceTypeParametersIn(IMaybeAntetype antetype)
        => antetype;

    public bool Equals(ITypeConstructor? other)
        => other is IMaybeAntetype that && Equals(that);
}
