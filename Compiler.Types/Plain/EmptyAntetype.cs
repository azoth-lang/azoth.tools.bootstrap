using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(NeverAntetype), typeof(VoidAntetype))]
public abstract class EmptyAntetype : NonGenericNominalAntetype, IAntetype
{
    /// <summary>
    /// Empty types aren't abstract, but they still can't be constructed.
    /// </summary>
    public override bool CanBeConstructed => false;

    public override SpecialTypeName Name { get; }

    private protected EmptyAntetype(SpecialTypeName name)
    {
        Name = name;
    }

    public sealed override string ToString() => Name.ToString();
}
