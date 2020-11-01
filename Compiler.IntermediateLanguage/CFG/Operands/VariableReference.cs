using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands
{
    public class VariableReference : Operand
    {
        public Variable Variable { get; }

        public VariableReference(Variable variable, TextSpan span)
            : base(span)
        {
            Variable = variable;
        }

        public override string ToString()
        {
            return Variable.ToString();
        }

        //public VariableReference AsOwn(TextSpan span)
        //{
        //    return new VariableReference(Variable, OldValueSemantics.Own, span);
        //}

        //public VariableReference AsBorrow()
        //{
        //    return ValueSemantics == OldValueSemantics.Borrow ? this : new VariableReference(Variable, OldValueSemantics.Borrow, Span);
        //}

        //public VariableReference AsAlias()
        //{
        //    return ValueSemantics == OldValueSemantics.Share ? this : new VariableReference(Variable, OldValueSemantics.Share, Span);
        //}
    }
}
