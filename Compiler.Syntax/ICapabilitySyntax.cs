using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface ICapabilitySyntax
{
    public static ICapabilitySyntax CreateDefault(TextSpan span)
    {
        Requires.That(span.Length == 0, nameof(span), "span must be empty");
        return Create(span, [], DeclaredCapability.Default);
    }
}
