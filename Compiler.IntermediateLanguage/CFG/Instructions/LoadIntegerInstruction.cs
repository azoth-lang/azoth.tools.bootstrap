using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class LoadIntegerInstruction : InstructionWithResult
    {
        public BigInteger Value { get; }
        public IntegerType Type { get; }

        public LoadIntegerInstruction(Place resultPlace, BigInteger value, IntegerType type, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Value = value;
            Type = type;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = LOAD[{Type}] {Value}";
        }
    }
}
