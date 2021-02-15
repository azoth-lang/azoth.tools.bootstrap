using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.IR.CFG;
using Azoth.Tools.Bootstrap.Compiler.IR.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.IL;

namespace Azoth.Tools.Bootstrap.Compiler.IR.Declarations
{
    public class FunctionIR : DeclarationIR
    {
        public new FunctionSymbol Symbol { get; }
        public FixedList<NamedParameterIR> Parameters { get; }
        public int Arity => Parameters.Count;
        public ControlFlowGraph ControlFlowGraph { get; }

        public FunctionIR(
            FunctionSymbol symbol,
            FixedList<NamedParameterIR> parameters,
            ControlFlowGraph controlFlowGraph)
            : base(symbol)
        {
            Symbol = symbol;
            Parameters = parameters;
            ControlFlowGraph = controlFlowGraph;
        }

        public FunctionIL ToIL()
        {
            return new FunctionIL(Parameters.Select(p => p.ToIL()).ToFixedList());
        }
    }
}
