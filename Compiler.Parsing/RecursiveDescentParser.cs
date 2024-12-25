using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public class RecursiveDescentParser
{
    protected CodeFile File { [DebuggerStepThrough] get; }
    protected ITokenIterator<IEssentialToken> Tokens { [DebuggerStepThrough] get; }

    public RecursiveDescentParser(ITokenIterator<IEssentialToken> tokens)
    {
        File = tokens.Context.File;
        Tokens = tokens;
    }

    protected void Add(Diagnostic diagnostic) => Tokens.Context.Diagnostics.Add(diagnostic);
}
