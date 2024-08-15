using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class ParameterSyntax : CodeSyntax, IParameterSyntax
{
    [DebuggerHidden]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IdentifierName? Name { get; }
    public bool Unused { get; }

    protected ParameterSyntax(TextSpan span, IdentifierName? name)
        : base(span)
    {
        Name = name;
        Unused = name?.Text.StartsWith('_') ?? false;
    }
}
