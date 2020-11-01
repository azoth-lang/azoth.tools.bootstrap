using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class ConvertInstruction : InstructionWithResult
    {
        public Operand Operand { get; }
        public NumericType FromType { get; }
        public NumericType ToType { get; }

        public ConvertInstruction(
            Place resultPlace,
            Operand operand,
            NumericType fromType,
            NumericType toType,
            TextSpan span,
            Scope scope)
            : base(resultPlace, span, scope)
        {
            Operand = operand;
            FromType = fromType;
            ToType = toType;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = CONVERT[{FromType},{ToType}] {Operand}";
        }
    }
}
