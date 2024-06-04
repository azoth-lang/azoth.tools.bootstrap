using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

/// <summary>
/// An antetype that is defined by its name.
/// </summary>
[Closed(typeof(NonGenericNominalAntetype), typeof(UserGenericNominalAntetype))]
public abstract class NominalAntetype : IAntetype
{
    public abstract IDeclaredAntetype Declared { get; }

    private protected NominalAntetype() { }
}
