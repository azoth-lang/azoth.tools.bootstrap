namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout
{
    internal class AzothObject
    {
        public readonly VTable VTable;
        public AzothObject(VTable vTable)
        {
            VTable = vTable;
        }
    }
}
