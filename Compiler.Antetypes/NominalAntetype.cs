using Compiler.Antetypes.Declared;

namespace Compiler.Antetypes;

/// <summary>
/// An antetype that is defined by its name.
/// </summary>
public abstract class NominalAntetype : IAntetype
{
    public IDeclaredAntetype Declared { get; }
}
