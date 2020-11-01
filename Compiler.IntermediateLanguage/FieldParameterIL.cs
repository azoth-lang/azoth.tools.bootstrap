using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage
{
    public sealed class FieldParameterIL : ParameterIL
    {
        public new FieldSymbol InitializeField { get; }

        public FieldParameterIL(FieldSymbol initializeField)
            : base(null, false, initializeField.DataType, initializeField)
        {
            InitializeField = initializeField;
        }
    }
}
