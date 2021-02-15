using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.IL
{
    public class FunctionIL
    {
        public FixedList<NamedParameterIL> Parameters { get; }
        public int Arity => Parameters.Count;

        public FunctionIL(FixedList<NamedParameterIL> parameters)
        {
            Parameters = parameters;
        }
    }
}
