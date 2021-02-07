using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.IL.Instructions
{
    [SuppressMessage("Design", "CA1028:Enum Storage should be Int32",
        Justification = "Unary Opcodes are 12-bit")]
    public enum UnaryOpcode : ushort
    {
        None = 0,
    }
}
