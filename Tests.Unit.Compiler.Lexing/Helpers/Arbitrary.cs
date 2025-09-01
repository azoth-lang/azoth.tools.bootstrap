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
    public static Arbitrary<PseudoToken> PsuedoToken() => Arb.From(GenPsuedoToken());

    public static Arbitrary<List<PseudoToken>> PsuedoTokenList()
        => Arb.From(GenPsuedoTokenList(), ShrinkTokens);

    /// <summary>
    /// Shrink a list of tokens by taking a prefix or suffix of the list.
    /// </summary>
    /// <remarks>The default list shrink removes items from the middle too.</remarks>
    private static IEnumerable<List<PseudoToken>> ShrinkTokens(List<PseudoToken> tokens)
    {
        foreach (var newSize in Arb.Shrink(tokens.Count).Where(size => size > 0))
        {
            yield return tokens.Take(newSize).ToList();
            yield return tokens.TakeLast(newSize).ToList();
        }
    }

    private static Gen<List<PseudoToken>> GenPsuedoTokenList()
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
    private static bool SeparateTokens(PseudoToken t1, PseudoToken t2)
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
                return t2.Text is "=" or "==" or "=/=" or "=>" or ":" or "::" or "::." or ".." or "..<";
            case "-":
                return t2.Text is "=" or "==" or "=/=" or "=>" or ">" or ">=";
            case "/":
                return t2.Text is "=" or "==" or "=/=" or "=>" or "*" or "*=" or "/" or "/="
                       || t2.TokenType == typeof(ICommentToken);
            case "@":
                return t2.Text is "==" or "=/=";
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
                return t2.Text is ":" or "::" or "::.";
            case "::":
                return t2.Text is "." or ".." or "..<";
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

    private static Gen<PseudoToken> GenPsuedoToken()
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

    private static Gen<PseudoToken> GenSymbol()
    {
        return Gen.Elements(Symbols.AsEnumerable())
                  .Select(Selector);

        static PseudoToken Selector(KeyValuePair<string, Type> pair)
        {
            // Type kinds and accessors are contextual keywords and have a value too
            if (pair.Value.IsAssignableTo(typeof(ITypeKindKeywordToken))
                || pair.Value.IsAssignableTo(typeof(IAccessorKeywordToken)))
                return new(pair.Value, pair.Key, pair.Key);
            return new(pair.Value, pair.Key);
        }
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

    private static Gen<PseudoToken> GenWhitespace()
    {
        var whitespaceChar = Gen.Elements(' ', '\t', '\n', '\r');
        return whitespaceChar.NonEmptyListOf()
            .Select(chars => new PseudoToken(typeof(IWhitespaceToken), string.Concat(chars)));
    }

    private static Gen<PseudoToken> GenComment()
    {
        // Covers both block comments and line comments
        // For line comments, end in newline requires escape sequences
        return GenRegex(@"(/\*(\**[^/])*\*/)|" + "(//.*[\r\n])")
            .Select(s => new PseudoToken(typeof(ICommentToken), s));
    }

    private static Gen<PseudoToken> GenBareIdentifier()
    {
        return GenRegex("[a-zA-Z_][a-zA-Z_0-9]*")
               .Where(s => !Symbols.ContainsKey(s)) // don't emit keywords
               .Select(s => new PseudoToken(typeof(IBareIdentifierToken), s, s));
    }

    private static Gen<PseudoToken> GenEscapedIdentifier()
    {
        return GenRegex(@"\\[a-zA-Z_0-9]+")
               .Where(s => !Symbols.ContainsKey(s)) // don't emit keywords
               .Select(s => new PseudoToken(typeof(IEscapedIdentifierToken), s, s[1..]));
    }

    private static Gen<PseudoToken> GenIdentifierString()
    {
        return GenRegex(@"\\\""([^\\""]|\\(r|n|0|t|\'|\""))*\""").Select(s =>
        {
            var value = s[2..^1].Unescape();
            return new PseudoToken(typeof(IIdentifierStringToken), s, value);
        });
    }

    private static Gen<PseudoToken> GenIntegerLiteral()
    {
        return Arb.Default.BigInt().Generator.Where(v => v > 0)
            .Select(v => new PseudoToken(typeof(IIntegerLiteralToken), v.ToString(CultureInfo.InvariantCulture), v));
    }

    private static Gen<PseudoToken> GenStringLiteral()
    {
        // @"""([^\\]|\\(r|n|0|t|'|""|\\|u\([0-9a-fA-F]{1,6}\)))*"""
        return GenRegex(@"\""([^\\""]|\\(r|n|0|t|\'|\""))*\""")
            .Select(s =>
            {
                var value = s[1..^1].Unescape();
                return new PseudoToken(typeof(IStringLiteralToken), s, value);
            });
    }

    public static readonly FixedDictionary<string, Type> Symbols = new Dictionary<string, Type>()
    {
        { "-", typeof(IMinusToken) },
        { "-=", typeof(IMinusEqualsToken) },
        { "->", typeof(IRightArrowToken) },
        { ",", typeof(ICommaToken) },
        { ";", typeof(ISemicolonToken) },
        { "::.", typeof(IColonColonDotToken) },
        { "::", typeof(IColonColonToken) },
        { ":", typeof(IColonToken) },
        { "??", typeof(IQuestionQuestionToken) },
        { "?.", typeof(IQuestionDotToken) },
        { "?", typeof(IQuestionToken) },
        { "..", typeof(IDotDotToken) },
        { "..<", typeof(IDotDotLessThanToken) },
        { ".", typeof(IDotToken) },
        { "(", typeof(IOpenParenToken) },
        { ")", typeof(ICloseParenToken) },
        { "[", typeof(IOpenBracketToken) },
        { "]", typeof(ICloseBracketToken) },
        { "{", typeof(IOpenBraceToken) },
        { "}", typeof(ICloseBraceToken) },
        { "@=/=", typeof(INotReferenceEqualsToken) },
        { "@==", typeof(IReferenceEqualsToken) },
        { "*", typeof(IAsteriskToken) },
        { "*=", typeof(IAsteriskEqualsToken) },
        { "/", typeof(ISlashToken) },
        { "/=", typeof(ISlashEqualsToken) },
        { "#", typeof(IHashToken) },
        { "+", typeof(IPlusToken) },
        { "+=", typeof(IPlusEqualsToken) },
        { "<:", typeof(ILessThanColonToken) },
        { "<..", typeof(ILessThanDotDotToken) },
        { "<..<", typeof(ILessThanDotDotLessThanToken) },
        { "<", typeof(ILessThanToken) },
        { "<=", typeof(ILessThanOrEqualToken) },
        { "=", typeof(IEqualsToken) },
        { "=/=", typeof(INotEqualToken) },
        { "==", typeof(IEqualsEqualsToken) },
        { "=>", typeof(IRightDoubleArrowToken) },
        { ">", typeof(IGreaterThanToken) },
        { ">=", typeof(IGreaterThanOrEqualToken) },
        { "|>", typeof(IRightTriangleToken) },
        //{ "@", typeof(IAtSignToken) },
        //{ "##", typeof(IHashHashToken) },
        //{ "^.", typeof(ICaretDotToken) },
        //{ "^", typeof(ICaretToken) },
        //{ "|", typeof(IPipeToken) },
        { "abstract", typeof(IAbstractKeywordToken) },
        { "aliasable", typeof(IAliasableKeywordToken) },
        { "and", typeof(IAndKeywordToken) },
        { "any", typeof(IAnyKeywordToken) },
        { "Any", typeof(IAnyTypeKeywordToken) },
        { "as!", typeof(IAsExclamationKeywordToken) },
        { "as?", typeof(IAsQuestionKeywordToken) },
        { "as", typeof(IAsKeywordToken) },
        { "async", typeof(IAsyncKeywordToken) },
        { "await", typeof(IAwaitKeywordToken) },
        { "base", typeof(IBaseKeywordToken) },
        { "bool", typeof(IBoolKeywordToken) },
        { "break", typeof(IBreakKeywordToken) },
        { "byte", typeof(IByteKeywordToken) },
        { "class", typeof(IClassKeywordToken) },
        { "const", typeof(IConstKeywordToken) },
        { "copy", typeof(ICopyKeywordToken) },
        { "do", typeof(IDoKeywordToken) },
        { "else", typeof(IElseKeywordToken) },
        { "false", typeof(IFalseKeywordToken) },
        { "fn", typeof(IFunctionKeywordToken) },
        { "foreach", typeof(IForeachKeywordToken) },
        { "freeze", typeof(IFreezeKeywordToken) },
        { "get", typeof(IGetKeywordToken) },
        { "go", typeof(IGoKeywordToken) },
        { "id", typeof(IIdKeywordToken) },
        { "if", typeof(IIfKeywordToken) },
        { "import", typeof(IImportKeywordToken) },
        { "in", typeof(IInKeywordToken) },
        { "ind", typeof(IIndependentKeywordToken) },
        { "init", typeof(IInitKeywordToken) },
        { "int", typeof(IIntKeywordToken) },
        { "int16", typeof(IInt16KeywordToken) },
        { "int32", typeof(IInt32KeywordToken) },
        { "int64", typeof(IInt64KeywordToken) },
        { "int8", typeof(IInt8KeywordToken) },
        { "is", typeof(IIsKeywordToken) },
        { "iso", typeof(IIsolatedKeywordToken) },
        { "lent", typeof(ILentKeywordToken) },
        { "let", typeof(ILetKeywordToken) },
        { "loop", typeof(ILoopKeywordToken) },
        { "move", typeof(IMoveKeywordToken) },
        { "mut", typeof(IMutableKeywordToken) },
        { "namespace", typeof(INamespaceKeywordToken) },
        { "never", typeof(INeverKeywordToken) },
        { "next", typeof(INextKeywordToken) },
        { "nint", typeof(INIntKeywordToken) },
        { "none", typeof(INoneKeywordToken) },
        { "nonwritable",typeof(INonwritableKeywordToken) },
        { "not", typeof(INotKeywordToken) },
        { "nuint", typeof(INUIntKeywordToken) },
        { "offset", typeof(IOffsetKeywordToken) },
        { "or", typeof(IOrKeywordToken) },
        { "out", typeof(IOutKeywordToken) },
        { "protected", typeof(IProtectedKeywordToken) },
        { "public", typeof(IPublicKeywordToken) },
        { "published", typeof(IPublishedKeywordToken) },
        { "read", typeof(IReadKeywordToken) },
        { "readable", typeof(IReadableKeywordToken) },
        { "return", typeof(IReturnKeywordToken) },
        { "safe", typeof(ISafeKeywordToken) },
        { "self", typeof(ISelfKeywordToken) },
        { "Self", typeof(ISelfTypeKeywordToken) },
        { "sendable", typeof(ISendableKeywordToken) },
        { "set", typeof(ISetKeywordToken) },
        { "shareable", typeof(IShareableKeywordToken) },
        { "size", typeof(ISizeKeywordToken) },
        { "struct", typeof(IStructKeywordToken) },
        { "temp", typeof(ITempKeywordToken) },
        { "trait", typeof(ITraitKeywordToken) },
        { "true", typeof(ITrueKeywordToken) },
        { "type", typeof(ITypeKeywordToken) },
        { "uint", typeof(IUIntKeywordToken) },
        { "uint16", typeof(IUInt16KeywordToken) },
        { "uint32", typeof(IUInt32KeywordToken) },
        { "uint64", typeof(IUInt64KeywordToken) },
        { "unsafe", typeof(IUnsafeKeywordToken) },
        { "value", typeof(IValueKeywordToken) },
        { "var", typeof(IVarKeywordToken) },
        { "void", typeof(IVoidKeywordToken) },
        { "while", typeof(IWhileKeywordToken) },
        //{ "_", typeof(IUnderscoreKeywordToken) },
        //{ "closed", typeof(IClosedKeywordToken) },
        //{ "delete", typeof(IDeleteKeywordToken) },
        //{ "ensures", typeof(IEnsuresKeywordToken) },
        //{ "explicit", typeof(IExplicitKeywordToken) },
        //{ "external", typeof(IExternalKeywordToken) },
        //{ "float16", typeof(IFloat16KeywordToken) },
        //{ "float32", typeof(IFloat32KeywordToken) },
        //{ "hides", typeof(IHidesKeywordToken) },
        //{ "implicit", typeof(IImplicitKeywordToken) },
        //{ "invariant", typeof(IInvariantKeywordToken) },
        //{ "match", typeof(IMatchKeywordToken) },
        //{ "may", typeof(IMayKeywordToken) },
        //{ "no", typeof(INoKeywordToken) },
        //{ "operator", typeof(IOperatorKeywordToken) },
        //{ "overrides", typeof(IOverridesKeywordToken) },
        //{ "params", typeof(IParamsKeywordToken) },
        //{ "requires", typeof(IRequiresKeywordToken) },
        //{ "throw", typeof(IThrowKeywordToken) },
        //{ "Type", typeof(ITypeTypeKeywordToken) },
        //{ "uninitialized", typeof(IUninitializedKeywordToken) },
        //{ "where", typeof(IWhereKeywordToken) },
    }.ToFixedDictionary();

    private readonly record struct AppendedToken(IEnumerable<PseudoToken> Items, PseudoToken? LastToken)
    {
        public static readonly AppendedToken Empty = new([], null);

        public AppendedToken Append(PseudoToken token) => new(ToEnumerable(), token);

        public IEnumerable<PseudoToken> ToEnumerable()
            => LastToken is { } token ? Items.Append(token) : Items;
    }
}
