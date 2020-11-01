using System.Numerics;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens
{
    public partial interface IIntegerLiteralToken : ILiteralToken
    {
        BigInteger Value { get; }
    }
}
