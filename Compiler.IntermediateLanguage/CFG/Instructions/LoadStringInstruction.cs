using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class LoadStringInstruction : InstructionWithResult
    {
        public static readonly Encoding Encoding = new UTF8Encoding(false);

        public string Value { get; }

        public LoadStringInstruction(Place resultPlace, string value, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Value = value;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = LOAD \"{Value.Escape()}\"";
        }
    }
}
