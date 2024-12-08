using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// A plain type that is a type variable.
/// </summary>
/// <remarks>Type variable can be used inside a type declaration only. Thus, they exit in a context
/// where no type arguments have been supplied yet. This includes any types the declaring type is
/// nested inside.</remarks>
[Closed(
    typeof(GenericParameterPlainType),
    typeof(AssociatedPlainType))]
public abstract class VariablePlainType : ConstructedOrVariablePlainType
{
    public sealed override TypeConstructor? TypeConstructor => null;
    public override TypeSemantics? Semantics => null;
    public sealed override bool AllowsVariance => false;

    public override IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType)
        => plainType;

    public sealed override string ToBareString() => ToString();
}
