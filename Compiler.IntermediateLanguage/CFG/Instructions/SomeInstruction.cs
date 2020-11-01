using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    /// <summary>
    /// Constructs a value of an optional type from the non-optional value
    /// </summary>
    public class SomeInstruction : InstructionWithResult
    {
        public Operand Operand { get; }
        public OptionalType Type { get; }

        public SomeInstruction(Place resultPlace, OptionalType type, Operand operand, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Type = type;
            Operand = operand;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = SOME[{Type}] {Operand}";
        }
    }
}
