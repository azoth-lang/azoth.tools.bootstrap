using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class NewObjectInstruction : InstructionWithResult
    {
        public ConstructorSymbol Constructor { get; }
        /// <summary>
        /// The type being constructed
        /// </summary>
        public ObjectType ConstructedType { get; }
        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count;

        public NewObjectInstruction(
            Place resultPlace,
            ConstructorSymbol constructor,
            ObjectType constructedType,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
            : base(resultPlace, span, scope)
        {
            Constructor = constructor;
            ConstructedType = constructedType;
            Arguments = arguments;
        }

        public override string ToInstructionString()
        {
            var result = ResultPlace != null ? $"{ResultPlace} = " : "";
            var arguments = string.Join(", ", Arguments);
            return $"{result}NEW {Constructor}({arguments})";
        }
    }
}
