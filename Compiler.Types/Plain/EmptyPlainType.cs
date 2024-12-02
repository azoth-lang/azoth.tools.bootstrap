using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(NeverPlainType), typeof(VoidPlainType))]
public abstract class EmptyPlainType : NonGenericNominalAntetype, IAntetype
{
    /// <summary>
    /// Empty types aren't abstract, but they still can't be instantiated.
    /// </summary>
    public override bool CanBeInstantiated => false;

    public override SpecialTypeName Name { get; }

    private protected EmptyPlainType(SpecialTypeName name)
    {
        Name = name;
    }

    public sealed override string ToString() => Name.ToString();
}
