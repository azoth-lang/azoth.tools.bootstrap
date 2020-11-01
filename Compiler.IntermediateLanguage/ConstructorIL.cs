using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage
{
    public class ConstructorIL : DeclarationIL, IInvocableDeclarationIL
    {
        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "NA")]
        bool IInvocableDeclarationIL.IsConstructor => true;
        public FixedList<ParameterIL> Parameters { get; }
        public int Arity => Parameters.Count;
        public FixedList<FieldInitializationIL> FieldInitializations { get; }
        public ControlFlowGraph IL { get; }
        public new ConstructorSymbol Symbol { get; }

        public ConstructorIL(
            ConstructorSymbol symbol,
            FixedList<ParameterIL> parameters,
            FixedList<FieldInitializationIL> fieldInitializations,
            ControlFlowGraph il)
            : base(true, symbol)
        {
            IL = il;
            FieldInitializations = fieldInitializations;
            Symbol = symbol;
            Parameters = parameters;
        }
    }
}
