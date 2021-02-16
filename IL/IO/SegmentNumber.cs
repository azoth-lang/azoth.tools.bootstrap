namespace Azoth.Tools.Bootstrap.IL.IO
{
    public enum SegmentNumber : ushort
    {
        Package = 0,
        PackageReferences = 1,
        StringConstants = 3,
        SymbolConstants = 4,
        Functions = 5,
        Types = 6,
        Classes = 7,
        EOF = 0xFFFF,
    }
}
