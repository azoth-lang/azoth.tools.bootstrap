using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IOpenBraceToken),
    typeof(ICloseBraceToken),
    typeof(IOpenParenToken),
    typeof(ICloseParenToken),
    typeof(ISemicolonToken),
    typeof(IColonToken),
    typeof(IColonColonToken),
    typeof(IColonColonDotToken),
    typeof(ICommaToken))]
public interface IPunctuationToken : IEssentialToken;

public partial interface IOpenBraceToken : IPunctuationToken;
public partial interface ICloseBraceToken : IPunctuationToken;
public partial interface IOpenParenToken : IPunctuationToken;
public partial interface ICloseParenToken : IPunctuationToken;
public partial interface ISemicolonToken : IPunctuationToken;
public partial interface IColonToken : IPunctuationToken;
public partial interface IColonColonToken : IPunctuationToken;
public partial interface IColonColonDotToken : IPunctuationToken;
public partial interface ICommaToken : IPunctuationToken;
