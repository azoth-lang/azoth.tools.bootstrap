using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Tests.Unit.Fakes;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Lexing.Fakes;

public static class FakeParseContext
{
    public static ParseContext For(string text)
        => new(FakeCodeFile.For(text), new DiagnosticCollectionBuilder());
}
