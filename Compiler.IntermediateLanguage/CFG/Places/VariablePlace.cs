using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places
{
    public class VariablePlace : Place
    {
        public Variable Variable { get; }

        public VariablePlace(Variable variable, TextSpan span)
            : base(span)
        {
            Variable = variable;
        }

        public override Operand ToOperand(TextSpan span)
        {
            // TODO is this the correct value semantics?
            return new VariableReference(Variable, span);
        }

        public override string ToString()
        {
            return Variable.ToString();
        }
    }
}
