using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage
{
    /// <summary>
    /// A field initialization in an initialization caused by a constructor parameter
    /// </summary>
    public class FieldInitializationIL
    {
        public FieldSymbol Field { get; }

        public FieldInitializationIL(FieldSymbol field)
        {
            Field = field;
        }
    }
}
