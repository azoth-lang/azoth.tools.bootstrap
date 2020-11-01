using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class FieldAccessInstruction : InstructionWithResult
    {
        public Operand Operand { get; }
        public FieldSymbol Field { get; }

        public FieldAccessInstruction(
            Place resultPlace,
            Operand operand,
            FieldSymbol field,
            TextSpan span,
            Scope scope)
            : base(resultPlace, span, scope)
        {
            Operand = operand;
            Field = field;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = LOAD[{Field.DataType}] {Operand}, {Field.ContainingSymbol}::{Field.Name}";
        }
    }
}
