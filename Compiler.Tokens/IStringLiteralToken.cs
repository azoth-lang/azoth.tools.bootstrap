namespace Azoth.Tools.Bootstrap.Compiler.Tokens
{
    public partial interface IStringLiteralToken : IEssentialToken
    {
        string Value { get; }
    }
}
