using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(typeof(NeverAntetype), typeof(VoidAntetype))]
public abstract class EmptyAntetype : NonGenericNominalAntetype
{
    public override bool IsAbstract => false;
    public override SpecialTypeName Name { get; }

    private protected EmptyAntetype(SpecialTypeName name)
    {
        Name = name;
    }

    public sealed override string ToString() => Name.ToString();
}
