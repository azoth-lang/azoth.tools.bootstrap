using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class CallVirtualInstruction : Instruction
    {
        public Place? ResultPlace { get; }
        public MethodSymbol Method { get; }
        public Operand Self { get; }
        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count;

        public CallVirtualInstruction(
            Place? resultPlace,
            Operand self,
            MethodSymbol method,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
            : base(span, scope)
        {
            ResultPlace = resultPlace;
            Self = self;
            Method = method;
            Arguments = arguments;
        }

        public CallVirtualInstruction(
            Operand self,
            MethodSymbol method,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
            : this(null, self, method, arguments, span, scope) { }

        public override string ToInstructionString()
        {
            var result = ResultPlace != null ? $"{ResultPlace} = " : "";
            var arguments = string.Join(", ", Arguments);
            return $"{result}CALL.VIRT({Self}).({arguments}) {Method}";
        }
    }
}
