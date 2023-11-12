using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Tests.Unit.Helpers;
using Fare;
using FsCheck;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Lexing.Helpers;

public static class Arbitrary
{
    public static Arbitrary<PsuedoToken> PsuedoToken() => Arb.From(GenPsuedoToken());

    public static Arbitrary<List<PsuedoToken>> PsuedoTokenList()
        => Arb.From(GenPsuedoTokenList(), ShrinkTokens);

    /// <summary>
    /// Shrink a list of tokens by taking a prefix or suffix of the list.
    /// </summary>
    /// <remarks>The default list shrink removes items from the middle too.</remarks>
    private static IEnumerable<List<PsuedoToken>> ShrinkTokens(List<PsuedoToken> tokens)
    {
        foreach (var newSize in Arb.Shrink(tokens.Count).Where(size => size > 0))
        {
            yield return tokens.Take(newSize).ToList();
            yield return tokens.TakeLast(newSize).ToList();
        }
    }

    private static Gen<List<PsuedoToken>> GenPsuedoTokenList()
        => Gen.Sized(size => GenAppendedPsuedoTokens(size)
                                .Select(tokens => tokens.ToEnumerable().ToList()));

    private static Gen<AppendedToken> GenAppendedPsuedoTokens(int length)
    {
        if (length <= 0)
            return Gen.Constant(AppendedToken.Empty);

        return GenAppendedPsuedoTokens(length - 1).SelectMany(tokens =>
            {
                var token = GenPsuedoToken();
                if (tokens.LastToken is { } lastToken)
                    token = token.Where(t => !SeparateTokens(lastToken, t));
                return token;
            }, (tokens, token) => tokens.Append(token));
    }

    /// <summary>
    /// Returns <see langword="true"/> if the two tokens need to be separated from each other.
    /// </summary>
    private static bool SeparateTokens(PsuedoToken t1, PsuedoToken t2)
    {
        switch (t1.Text)
        {
            case ".":
            case "^":
                return t2.Text is "." or ".." or "..<";
            case "+":
            case ">":
                return t2.Text is "=" or "==" or "=/=" or "=>";
            case "*":
                return t2.Text is "=" or "==" or "=/=" or "=>" or "*=";
            case "<":
                return t2.Text is "=" or "==" or "=/=" or "=>" or ":" or "::." or ".." or "..<";
            case "-":
                return t2.Text is "=" or "==" or "=/=" or "=>" or ">" or ">=";
            case "/":
                return t2.Text is "=" or "==" or "=/=" or "=>" or "*" or "*=" or "/" or "/="
                       || t2.TokenType == typeof(ICommentToken);
            case "=":
                return t2.Text is "=" or "==" or "=/=" or "=>" or "/=" or ">" or ">=";
            case "?":
                return t2.Text is "." or ".." or "..<" or "?" or "?." or "??";
            case "..":
            case "<..":
                return t2.Text is "<" or "<=" or "<:" or "<.." or "<..<";
            case "#":
                return t2.Text is "#" or "##" or "(" or "[" or "{";
            case ":":
                // TODO actually ':',':' is fine. It is really the three token sequence ':',':','.' that is the problem
                return t2.Text == ":";
            case "as"
                when t2.Text is "?" or "?." or "??":
                return true;
            default:
                if (typeof(IKeywordToken).IsAssignableFrom(t1.TokenType)
                    || typeof(IIdentifierToken).IsAssignableFrom(t1.TokenType))
                    return typeof(IBareIdentifierToken).IsAssignableFrom(t2.TokenType)
                           || typeof(IKeywordToken).IsAssignableFrom(t2.TokenType)
                           || t2.TokenType.IsAssignableTo(typeof(IIntegerLiteralToken));
                if (t1.TokenType == typeof(IIntegerLiteralToken))
                    return t2.TokenType == typeof(IIntegerLiteralToken);
                if (t1.TokenType == typeof(IWhitespaceToken))
                    return t2.TokenType == typeof(IWhitespaceToken);
                return false;
        }
    }

    private static Gen<PsuedoToken> GenPsuedoToken()
    {
        return Gen.Frequency(
            GenSymbol().WithWeight(20),
            GenWhitespace().WithWeight(10),
            GenComment().WithWeight(5),
            GenBareIdentifier().WithWeight(10),
            GenEscapedIdentifier().WithWeight(5),
            GenIdentifierString().WithWeight(5),
            GenIntegerLiteral().WithWeight(5),
            GenStringLiteral().WithWeight(5));
    }

    private static Gen<PsuedoToken> GenSymbol()
    {
        return Gen.Elements(Symbols.AsEnumerable())
                  .Select(item => new PsuedoToken(item.Value, item.Key));
    }

    private static Gen<string> GenRegex(string pattern)
    {
        // TODO use ScaleSize instead
        return Gen.Sized(size =>
        {
            size = Math.Max(size, 1);
            return Arb.Default.Int32().Generator.Select(seed => GenRegex(pattern, seed)).Resize(size);
        });
    }

    private static string GenRegex(string pattern, int seed)
    {
        var random = new System.Random(seed);
        // TODO don't reparse the pattern each time
        var xegar = new Xeger(pattern, random);
        return xegar.Generate();
    }

    private static Gen<PsuedoToken> GenWhitespace()
    {
        var whitespaceChar = Gen.Elements(' ', '\t', '\n', '\r');
        return whitespaceChar.NonEmptyListOf()
            .Select(chars => new PsuedoToken(typeof(IWhitespaceToken), string.Concat(chars)));
    }

    private static Gen<PsuedoToken> GenComment()
    {
        // Covers both block comments and line comments
        // For line comments, end in newline requires escape sequences
        return GenRegex(@"(/\*(\**[^/])*\*/)|" + "(//.*[\r\n])")
            .Select(s => new PsuedoToken(typeof(ICommentToken), s));
    }

    private static Gen<PsuedoToken> GenBareIdentifier()
    {
        return GenRegex("[a-zA-Z_][a-zA-Z_0-9]*")
               .Where(s => !Symbols.ContainsKey(s)) // don't emit keywords
               .Select(s => new PsuedoToken(typeof(IBareIdentifierToken), s, s));
    }

    private static Gen<PsuedoToken> GenEscapedIdentifier()
    {
        return GenRegex(@"\\[a-zA-Z_0-9]+")
               .Where(s => !Symbols.ContainsKey(s)) // don't emit keywords
               .Select(s => new PsuedoToken(typeof(IEscapedIdentifierToken), s, s[1..]));
    }

    private static Gen<PsuedoToken> GenIdentifierString()
    {
        return GenRegex(@"\\\""([^\\""]|\\(r|n|0|t|\'|\""))*\""").Select(s =>
        {
            var value = s[2..^1].Unescape();
            return new PsuedoToken(typeof(IIdentifierStringToken), s, value);
        });
    }

    private static Gen<PsuedoToken> GenIntegerLiteral()
    {
        return Arb.Default.BigInt().Generator.Where(v => v > 0)
            .Select(v => new PsuedoToken(typeof(IIntegerLiteralToken), v.ToString(CultureInfo.InvariantCulture), v));
    }

    private static Gen<PsuedoToken> GenStringLiteral()
    {
        // @"""([^\\]|\\(r|n|0|t|'|""|\\|u\([0-9a-fA-F]{1,6}\)))*"""
        return GenRegex(@"\""([^\\""]|\\(r|n|0|t|\'|\""))*\""")
            .Select(s =>
            {
                var value = s[1..^1].Unescape();
                return new PsuedoToken(typeof(IStringLiteralToken), s, value);
            });
    }

    public static readonly FixedDictionary<string, Type> Symbols = new Dictionary<string, Type>()
    {
        { "{", typeof(IOpenBraceToken) },
        { "}", typeof(ICloseBraceToken) },
        { "(", typeof(IOpenParenToken) },
        { ")", typeof(ICloseParenToken) },
        { "[", typeof(IOpenBracketToken) },
        { "]", typeof(ICloseBracketToken) },
        { ";", typeof(ISemicolonToken) },
        { ",", typeof(ICommaToken) },
        { ".", typeof(IDotToken) },
        { "::.", typeof(IColonColonDotToken) },
        { "..", typeof(IDotDotToken) },
        { "<..", typeof(ILessThanDotDotToken) },
        { "..<", typeof(IDotDotLessThanToken) },
        { "<..<", typeof(ILessThanDotDotLessThanToken) },
        { ":", typeof(IColonToken) },
        { "<:", typeof(ILessThanColonToken) },
        { "?", typeof(IQuestionToken) },
        { "?.", typeof(IQuestionDotToken) },
        { "??", typeof(IQuestionQuestionToken) },
        //{ "|", typeof(IPipeToken) },
        { "→", typeof(IRightArrowToken) },
        { "->", typeof(IRightArrowToken) },
        { "#", typeof(IHashToken) },
        //{ "##", typeof(IHashHashToken) },
        //{ "@", typeof(IAtSignToken) },
        //{ "^", typeof(ICaretToken) },
        //{ "^.", typeof(ICaretDotToken) },
        { "+", typeof(IPlusToken) },
        { "-", typeof(IMinusToken) },
        { "*", typeof(IAsteriskToken) },
        { "/", typeof(ISlashToken) },
        { "=", typeof(IEqualsToken) },
        { "==", typeof(IEqualsEqualsToken) },
        { "≠", typeof(INotEqualToken) },
        { "=/=", typeof(INotEqualToken) },
        { ">", typeof(IGreaterThanToken) },
        { "≥", typeof(IGreaterThanOrEqualToken) },
        { ">=", typeof(IGreaterThanOrEqualToken) },
        { "<", typeof(ILessThanToken) },
        { "≤", typeof(ILessThanOrEqualToken) },
        { "<=", typeof(ILessThanOrEqualToken) },
        { "+=", typeof(IPlusEqualsToken) },
        { "-=", typeof(IMinusEqualsToken) },
        { "*=", typeof(IAsteriskEqualsToken) },
        { "/=", typeof(ISlashEqualsToken) },
        { "⇒", typeof(IRightDoubleArrowToken) },
        { "=>", typeof(IRightDoubleArrowToken) },
        { "published", typeof(IPublishedKeywordToken) },
        { "public", typeof(IPublicKeywordToken) },
        //{ "protected", typeof(IProtectedKeywordToken) },
        { "let", typeof(ILetKeywordToken) },
        { "var", typeof(IVarKeywordToken) },
        { "void", typeof(IVoidKeywordToken) },
        { "int", typeof(IIntKeywordToken) },
        { "int8", typeof(IInt8KeywordToken) },
        { "int16", typeof(IInt16KeywordToken) },
        { "int32", typeof(IInt32KeywordToken) },
        { "int64", typeof(IInt64KeywordToken) },
        { "uint", typeof(IUIntKeywordToken) },
        { "byte", typeof(IByteKeywordToken) },
        { "uint16", typeof(IUInt16KeywordToken) },
        { "uint32", typeof(IUInt32KeywordToken) },
        { "uint64", typeof(IUInt64KeywordToken) },
        { "bool", typeof(IBoolKeywordToken) },
        { "return", typeof(IReturnKeywordToken) },
        { "class", typeof(IClassKeywordToken) },
        { "new", typeof(INewKeywordToken) },
        //{ "delete", typeof(IDeleteKeywordToken) },
        { "namespace", typeof(INamespaceKeywordToken) },
        { "using", typeof(IUsingKeywordToken) },
        { "foreach", typeof(IForeachKeywordToken) },
        { "in", typeof(IInKeywordToken) },
        { "if", typeof(IIfKeywordToken) },
        { "else", typeof(IElseKeywordToken) },
        { "not", typeof(INotKeywordToken) },
        { "and", typeof(IAndKeywordToken) },
        { "or", typeof(IOrKeywordToken) },
        //{ "struct", typeof(IStructKeywordToken) },
        //{ "enum", typeof(IEnumKeywordToken) },
        { "size", typeof(ISizeKeywordToken) },
        { "unsafe", typeof(IUnsafeKeywordToken) },
        { "safe", typeof(ISafeKeywordToken) },
        //{ "base", typeof(IBaseKeywordToken) },
        { "fn", typeof(IFunctionKeywordToken) },
        //{ "Self", typeof(ISelfTypeKeywordToken) },
        //{ "init", typeof(IInitKeywordToken) },
        { "iso", typeof(IIsolatedKeywordToken) },
        { "const", typeof(IConstKeywordToken)},
        { "id", typeof(IIdKeywordToken) },
        { "lent", typeof(ILentKeywordToken) },
        { "self", typeof(ISelfKeywordToken) },
        //{ "Type", typeof(ITypeKeywordToken) },
        { "true", typeof(ITrueKeywordToken) },
        { "false", typeof(IFalseKeywordToken) },
        { "mut", typeof(IMutableKeywordToken) },
        //{ "params", typeof(IParamsKeywordToken) },
        //{ "may", typeof(IMayKeywordToken) },
        //{ "no", typeof(INoKeywordToken) },
        //{ "throw", typeof(IThrowKeywordToken) },
        //{ "ref", typeof(IRefKeywordToken) },
        { "abstract", typeof(IAbstractKeywordToken) },
        //{ "get", typeof(IGetKeywordToken) },
        //{ "set", typeof(ISetKeywordToken) },
        //{ "requires", typeof(IRequiresKeywordToken) },
        //{ "ensures", typeof(IEnsuresKeywordToken) },
        //{ "invariant", typeof(IInvariantKeywordToken) },
        //{ "where", typeof(IWhereKeywordToken) },
        //{ "uninitialized", typeof(IUninitializedKeywordToken) },
        { "none", typeof(INoneKeywordToken) },
        //{ "operator", typeof(IOperatorKeywordToken) },
        //{ "implicit", typeof(IImplicitKeywordToken) },
        //{ "explicit", typeof(IExplicitKeywordToken) },
        { "move", typeof(IMoveKeywordToken) },
        { "copy", typeof(ICopyKeywordToken) },
        //{ "match", typeof(IMatchKeywordToken) },
        { "loop", typeof(ILoopKeywordToken) },
        { "while", typeof(IWhileKeywordToken) },
        { "break", typeof(IBreakKeywordToken) },
        { "next", typeof(INextKeywordToken) },
        //{ "override", typeof(IOverrideKeywordToken) },
        { "as", typeof(IAsKeywordToken) },
        { "as!", typeof(IAsExclamationKeywordToken) },
        { "as?", typeof(IAsQuestionKeywordToken) },
        { "Any", typeof(IAnyKeywordToken) },
        { "never", typeof(INeverKeywordToken) },
        { "trait", typeof(ITraitKeywordToken) },
        //{ "float16", typeof(IFloat16KeywordToken) },
        //{ "float32", typeof(IFloat32KeywordToken) },
        { "offset", typeof(IOffsetKeywordToken) },
        //{ "_", typeof(IUnderscoreKeywordToken) },
        //{ "external", typeof(IExternalKeywordToken) },
        { "freeze", typeof(IFreezeKeywordToken) },
        { "is", typeof(IIsKeywordToken) },
    }.ToFixedDictionary();

    private readonly record struct AppendedToken(IEnumerable<PsuedoToken> Items, PsuedoToken? LastToken)
    {
        public static readonly AppendedToken Empty = new(Enumerable.Empty<PsuedoToken>(), null);

        public AppendedToken Append(PsuedoToken token) => new(ToEnumerable(), token);

        public IEnumerable<PsuedoToken> ToEnumerable()
            => LastToken is { } token ? Items.Append(token) : Items;
    }
}
