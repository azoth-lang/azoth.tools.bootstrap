using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.IL
{
    public class FunctionIL
    {
        public FunctionIL(FixedList<ParameterIL> parameters)
        {
            Parameters = parameters;
        }
        public FixedList<ParameterIL> Parameters { get; }
        public int Arity => Parameters.Count;
    }
}
