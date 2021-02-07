using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.IL.Instructions
{
    [SuppressMessage("Design", "CA1028:Enum Storage should be Int32",
        Justification = "Short Opcodes are stored as bytes")]
    public enum ShortOpcode : byte
    {
        None = 0,
    }
}
