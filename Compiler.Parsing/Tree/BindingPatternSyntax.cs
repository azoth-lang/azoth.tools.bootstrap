using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class BindingPatternSyntax : CodeSyntax, IBindingPatternSyntax
{
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    TextSpan ILocalBindingSyntax.NameSpan => Span;

    public BindingPatternSyntax(TextSpan span, bool isMutableBinding, IdentifierName name)
        : base(span)
    {
        IsMutableBinding = isMutableBinding;
        Name = name;
    }

    public override string ToString() => Name.ToString();
}
