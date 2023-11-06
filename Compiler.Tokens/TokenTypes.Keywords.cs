using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

public static partial class TokenTypes
{
    // Must be before KeywordFactories because it is used in the construction of it
    private static readonly int KeywordTokenLength = "KeywordToken".Length;

    public static readonly FixedDictionary<string, Func<TextSpan, IKeywordToken>> KeywordFactories
        = BuildKeywordFactories();

    public static readonly IReadOnlyCollection<string> Keywords = KeywordFactories.Keys.ToHashSet();

    private static FixedDictionary<string, Func<TextSpan, IKeywordToken>> BuildKeywordFactories()
    {
        var factories = new Dictionary<string, Func<TextSpan, IKeywordToken>>();

        foreach (var tokenType in Keyword)
        {
            var tokenTypeName = tokenType.Name;
            string keyword = tokenTypeName switch
            {
                // Some exceptions to the normal rule
                nameof(FunctionKeywordToken) => "fn",
                //nameof(SelfTypeKeywordToken) => "Self",
                nameof(IsolatedKeywordToken) => "iso",
                nameof(MutableKeywordToken) => "mut",
                nameof(ExclusivelyMutableKeywordToken) => "xmut",
                nameof(AnyKeywordToken) => "Any",
                //nameof(TypeKeywordToken) => "Type",
                nameof(AsExclamationKeywordToken) => "as!",
                nameof(AsQuestionKeywordToken) => "as?",
                "UnderscoreKeywordToken" => "_",
                _ => tokenTypeName[..^KeywordTokenLength]
                                  .ToLower(CultureInfo.InvariantCulture)
            };
            var factory = CompileFactory<IKeywordToken>(tokenType);
            factories.Add(keyword, factory);
        }
        return factories.ToFixedDictionary();
    }

    private static Func<TextSpan, T> CompileFactory<T>(Type tokenType)
        where T : IToken
    {
        var spanParam = Expression.Parameter(typeof(TextSpan), "span");
        var newExpression = Expression.New(tokenType.GetConstructor(new[] { typeof(TextSpan) })!, spanParam);
        var factory =
            Expression.Lambda<Func<TextSpan, T>>(
                newExpression, spanParam);
        return factory.Compile();
    }
}
