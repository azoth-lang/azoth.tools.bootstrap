using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class TypeSyntax : CodeSyntax, ITypeSyntax
{
    protected TypeSyntax(TextSpan span)
        : base(span)
    {
    }
}
