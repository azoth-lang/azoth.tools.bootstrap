namespace Azoth.Tools.Bootstrap.IL.Instructions
{
    public enum NullaryOpcode : ushort
    {
        ConstNone = 0,
        ConstFalse = 0x002,
        ConstTrue = 0x003,
        ParamU8 = 0x102,
        ParamI8 = 0x103,
        ParamU16 = 0x104,
        ParamI16 = 0x105,
        ParamU32 = 0x106,
        ParamI32 = 0x107,
        ParamU64 = 0x108,
        ParamI64 = 0x109,
        ParamU128 = 0x10A,
        ParamI128 = 0x10B,
        ParamUInt = 0x10C,
        ParamInt = 0x10D,
        ParamSize = 0x10E,
        ParamOffset = 0x10F,
        ParamBool = 0x120,
        ReturnVoid = 0x200,
    }
}
