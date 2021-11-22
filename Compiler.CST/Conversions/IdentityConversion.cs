using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions
{
    /// <summary>
    /// A non-conversion that leaves the type unchanged. This is used as an underlying
    /// conversion for other conversions when no further conversion is needed.
    /// </summary>
    public class IdentityConversion : Conversion
    {
        public IdentityConversion(DataType to)
            : base(to)
        {
        }
    }
}
