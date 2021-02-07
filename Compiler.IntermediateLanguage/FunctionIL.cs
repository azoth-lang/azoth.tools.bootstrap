using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage
{
    public class FunctionIL : DeclarationIL, IInvocableDeclarationIL
    {
        public bool IsExternal { get; }

        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types",
            Justification = "NA")]
        bool IInvocableDeclarationIL.IsConstructor => false;

        public FixedList<ParameterIL> Parameters { get; }
        public int Arity => Parameters.Count;
        public new FunctionSymbol Symbol { get; }
        public ControlFlowGraph IL { get; }

        public FunctionIL(
            bool isExternal,
            bool isMember,
            FunctionSymbol symbol,
            FixedList<ParameterIL> parameters,
            ControlFlowGraph il)
            : base(isMember, symbol)
        {
            Parameters = parameters;
            Symbol = symbol;
            IL = il;
            IsExternal = isExternal;
        }
    }
}
