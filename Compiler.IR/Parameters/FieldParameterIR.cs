using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.IR.Parameters
{
    public class FieldParameterIR : ConstructorParameterIR
    {
        public FieldSymbol InitializeField { get; }

        public FieldParameterIR(FieldSymbol initializeField)
        {
            InitializeField = initializeField;
        }
    }
}
