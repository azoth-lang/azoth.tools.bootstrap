using Azoth.Tools.Bootstrap.Compiler.IR.CFG;
using Azoth.Tools.Bootstrap.Compiler.IR.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IR.Declarations
{
    public class MethodIR : MemberIR
    {
        public new MethodSymbol Symbol { get; }
        public ParameterIR SelfParameter { get; }
        public FixedList<NamedParameterIR> Parameters { get; }
        public int Arity => Parameters.Count;
        public ControlFlowGraph ControlFlowGraph { get; }

        public MethodIR(
            MethodSymbol symbol,
            SelfParameterIR selfParameter,
            FixedList<NamedParameterIR> parameters,
            ControlFlowGraph controlFlowGraph)
            : base(symbol)
        {
            Symbol = symbol;
            SelfParameter = selfParameter;
            Parameters = parameters;
            ControlFlowGraph = controlFlowGraph;
        }
    }
}
