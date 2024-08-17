using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface IBindingPatternSyntax
{
    public static IBindingPatternSyntax Create(
        TextSpan span,
        bool isMutableBinding,
        IdentifierName name)
        // TODO support AG defining nameSpan == span so it doesn't have to be passed twice
        => Create(span, isMutableBinding, span, name);
}
