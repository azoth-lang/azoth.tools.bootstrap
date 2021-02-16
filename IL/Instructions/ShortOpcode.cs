using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.IL.Instructions
{
    [SuppressMessage("Design", "CA1028:Enum Storage should be Int32",
        Justification = "Short Opcodes are stored as bytes")]
    public enum ShortOpcode : byte
    {
        None = 0,
        ConstU8 = 0x01,
        ConstI8 = 0x02,
        ConstU16 = 0x03,
        ConstI16 = 0x04,
        ConstU32 = 0x05,
        ConstI32 = 0x06,
        ConstStr = 0x07,
        ConstSym = 0x08,
        Const = 0x09,
        Param = 0x52,
    }
}
