using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class ParameterSyntax : CodeSyntax, IParameterSyntax
{
    [DebuggerHidden]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IdentifierName? Name { get; }

    protected ParameterSyntax(TextSpan span, IdentifierName? name)
        : base(span)
    {
        Name = name;
    }
}
