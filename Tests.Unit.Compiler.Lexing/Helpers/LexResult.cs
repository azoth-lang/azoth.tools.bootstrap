using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Lexing.Helpers;

public class LexResult
{
    public CodeFile File { get; }
    public IFixedList<IToken> Tokens { get; }
    public DiagnosticsCollection Diagnostics { get; }

    public LexResult(ITokenIterator<IToken> iterator)
    {
        var tokens = new List<IToken>();
        do
        {
            tokens.Add(iterator.Current);
        } while (iterator.Next());

        File = iterator.Context.File;
        Tokens = tokens.ToFixedList();
        Diagnostics = iterator.Context.Diagnostics.Build();
    }

    public IToken AssertSingleToken()
    {
        Assert.True(2 == Tokens.Count, $"Expected token count {1}, was {Tokens.Count - 1} (excluding EOF)");
        var eof = Assert.IsAssignableFrom<IEndOfFileToken>(Tokens[^1]);
        Assert.Equal(new TextSpan(Tokens[^2].Span.End, 0), eof.Span);
        return Tokens[0];
    }

    public void AssertTokens(int expectedCount)
    {
        Assert.Equal(expectedCount + 1, Tokens.Count);
    }

    public void AssertNoDiagnostics()
    {
        Assert.Empty(Diagnostics);
    }

    public Diagnostic AssertSingleDiagnostic()
    {
        return Assert.Single(Diagnostics);
    }

    public void AssertDiagnostics(int expectedCount)
    {
        Assert.Equal(expectedCount, Diagnostics.Count);
    }

    public string TokensToString()
    {
        return string.Concat(Tokens.Select(t => t.Text(File.Code)));
    }

    public IFixedList<PseudoToken> ToPsuedoTokens()
    {
        return Tokens.Select(t => PseudoToken.For(t, File.Code)).ToFixedList();
    }
}
