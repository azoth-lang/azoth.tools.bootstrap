using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.IL
{
    [SuppressMessage("Design", "CA1028:Enum Storage should be Int32",
        Justification = "OpCodes are stored as bytes")]
    public enum OpCode : byte
    {
        None
    }
}
