using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class FunctionAntetype : IAntetype
{
    public IFixedList<INonVoidAntetype> Parameters { get; }
    public IAntetype Return { get; }

    public FunctionAntetype(IEnumerable<INonVoidAntetype> parameters, IAntetype returnAntetype)
    {
        Return = returnAntetype;
        Parameters = parameters.ToFixedList();
    }
}
