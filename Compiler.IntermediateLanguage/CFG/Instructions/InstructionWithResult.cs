using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    [Closed(
        typeof(AssignmentInstruction),
        typeof(CompareInstruction),
        typeof(ConvertInstruction),
        typeof(FieldAccessInstruction),
        typeof(LoadBoolInstruction),
        typeof(LoadIntegerInstruction),
        typeof(LoadStringInstruction),
        typeof(LoadNoneInstruction),
        typeof(NegateInstruction),
        typeof(NewObjectInstruction),
        typeof(NumericInstruction),
        typeof(SomeInstruction),
        typeof(BooleanLogicInstruction))]
    public abstract class InstructionWithResult : Instruction
    {
        public Place ResultPlace { get; }

        protected InstructionWithResult(Place resultPlace, TextSpan span, Scope scope)
            : base(span, scope)
        {
            ResultPlace = resultPlace;
        }
    }
}
