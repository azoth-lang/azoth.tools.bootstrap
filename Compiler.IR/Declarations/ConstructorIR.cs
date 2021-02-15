using Azoth.Tools.Bootstrap.Compiler.IR.CFG;
using Azoth.Tools.Bootstrap.Compiler.IR.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IR.Declarations
{
    public class ConstructorIR : DeclarationIR
    {
        public new ConstructorSymbol Symbol { get; }
        public FixedList<ConstructorParameterIR> Parameters { get; }
        public int Arity => Parameters.Count;
        public ControlFlowGraph ControlFlowGraph { get; }

        public ConstructorIR(
            ConstructorSymbol symbol,
            FixedList<ConstructorParameterIR> parameters,
            ControlFlowGraph controlFlowGraph)
            : base(symbol)
        {
            Symbol = symbol;
            Parameters = parameters;
            ControlFlowGraph = controlFlowGraph;
        }
    }
}
