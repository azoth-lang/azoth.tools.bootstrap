using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage
{
    public class MethodDeclarationIL : DeclarationIL, IInvocableDeclarationIL
    {
        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types",
            Justification = "NA")]
        bool IInvocableDeclarationIL.IsConstructor => false;
        public ParameterIL SelfParameter { get; }
        public FixedList<ParameterIL> Parameters { get; }
        public int Arity => Parameters.Count;
        public new MethodSymbol Symbol { get; }
        public ControlFlowGraph? IL { get; }

        public MethodDeclarationIL(
            MethodSymbol symbol,
            ParameterIL selfParameter,
            FixedList<ParameterIL> parameters,
            ControlFlowGraph? il)
            : base(true, symbol)
        {
            SelfParameter = selfParameter;
            Parameters = parameters;
            Symbol = symbol;
            IL = il;
        }
    }
}
