using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.IL.Instructions
{
    [SuppressMessage("Design", "CA1028:Enum Storage should be Int32",
        Justification = "Unary Opcodes are 12-bit")]
    public enum UnaryOpcode : ushort
    {
        None = 0,
        Return = 0x002,
        ConvertToU8 = 0x102,
        ConvertToI8 = 0x103,
        ConvertToU16 = 0x104,
        ConvertToI16 = 0x105,
        ConvertToU32 = 0x106,
        ConvertToI32 = 0x107,
        ConvertToU64 = 0x108,
        ConvertToI64 = 0x109,
        ConvertToU128 = 0x10A,
        ConvertToI128 = 0x10B,
        ConvertToUInt = 0x10C,
        ConvertToInt = 0x10D,
        ConvertToSize = 0x10E,
        ConvertToOffset = 0x10F,
        ConvertToBool = 0x120,
    }
}
