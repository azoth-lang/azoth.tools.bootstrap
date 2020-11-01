namespace Azoth.Tools.Bootstrap.Interpreter.MemoryLayout
{
    public readonly struct AzothReference
    {
        public readonly AzothObject Object;
        public readonly VTableRef VTableRef;

        public AzothReference(AzothObject o, VTableRef vTable)
        {
            Object = o;
            VTableRef = vTable;
        }
    }
}
