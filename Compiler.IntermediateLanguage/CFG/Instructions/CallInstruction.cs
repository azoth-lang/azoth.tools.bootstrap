using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class CallInstruction : Instruction
    {
        public Place? ResultPlace { get; }
        public InvocableSymbol Function { get; }
        public Operand? Self { get; }
        public bool IsMethodCall => !(Self is null);
        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count;

        private CallInstruction(Place? resultPlace, Operand? self, InvocableSymbol function, FixedList<Operand> arguments, TextSpan span, Scope scope)
            : base(span, scope)
        {
            ResultPlace = resultPlace;
            Self = self;
            Function = function;
            Arguments = arguments;
        }

        public static CallInstruction ForFunction(
            Place resultPlace,
            FunctionSymbol function,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
        {
            return new CallInstruction(resultPlace, null, function, arguments, span, scope);
        }

        public static CallInstruction ForFunction(
            FunctionSymbol function,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
        {
            return new CallInstruction(null, null, function, arguments, span, scope);
        }

        public static CallInstruction ForMethod(
            Place resultPlace,
            Operand self,
            MethodSymbol method,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
        {
            return new CallInstruction(resultPlace, self, method, arguments, span, scope);
        }

        public static CallInstruction ForMethod(
            Operand self,
            MethodSymbol method,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
        {
            return new CallInstruction(null, self, method, arguments, span, scope);
        }

        public override string ToInstructionString()
        {
            var result = ResultPlace != null ? $"{ResultPlace} = " : "";
            var selfArgument = Self?.ToString();
            if (!(selfArgument is null))
                selfArgument =  $"({selfArgument}).";
            var arguments = string.Join(", ", Arguments);
            var callType = IsMethodCall ? "METHOD" : "FN";
            return $"{result}CALL.{callType}{selfArgument}({arguments}) {Function}";
        }
    }
}
