using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing
{
    [SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "Not Plural")]
    public enum ParseAs
    {
        Expression = 1,
        Statement
    }
}
