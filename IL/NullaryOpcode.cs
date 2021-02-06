using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.IL
{
    [SuppressMessage("Design", "CA1028:Enum Storage should be Int32",
        Justification = "Nullary Opcodes are 12-bit")]
    public enum NullaryOpcode : ushort
    {
        None = 0,
    }
}
