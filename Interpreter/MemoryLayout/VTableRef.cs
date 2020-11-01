namespace Azoth.Tools.Bootstrap.Interpreter.MemoryLayout
{
    public readonly struct VTableRef
    {
        private readonly int index;

        public VTableRef(int index)
        {
            this.index = index;
        }
    }
}
