using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(typeof(NeverAntetype), typeof(VoidAntetype))]
public abstract class EmptyAntetype : NonGenericNominalAntetype
{
    public SpecialTypeName Name { get; }

    protected EmptyAntetype(SpecialTypeName name)
    {
        Name = name;
    }

    public sealed override IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        => antetype;

    public sealed override string ToString() => Name.ToString();
}
