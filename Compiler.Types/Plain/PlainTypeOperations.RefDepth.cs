namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class PlainTypeOperations
{
    public static int RefDepth(this IMaybePlainType plainType)
        => plainType is RefPlainType t ? 1 + t.Referent.RefDepth() : 0;
}
