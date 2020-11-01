using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places
{
    public class FieldPlace : Place
    {
        public Operand Target { get; }
        public FieldSymbol Field { get; }

        public FieldPlace(Operand target, FieldSymbol field, TextSpan span)
            : base(span)
        {
            Target = target;
            Field = field;
        }

        public override Operand ToOperand(TextSpan span)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"({Target}).{Field.Name}";
        }
    }
}
