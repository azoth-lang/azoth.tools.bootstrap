using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// An plain type that is a type variable.
/// </summary>
[Closed(
    typeof(GenericParameterPlainType),
    typeof(SelfPlainType))]
public abstract class VariablePlainType : NamedPlainType, INonVoidAntetype
{
    public sealed override ITypeConstructor? TypeConstructor => null;
    public TypeSemantics? Semantics => null;
    public sealed override bool AllowsVariance => false;

    public override IMaybeAntetype ReplaceTypeParametersIn(IMaybeAntetype antetype)
        => antetype;
}
