using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface ICapabilitySetSyntax
{
    public static ICapabilitySetSyntax CreateImplicitAliasable(TextSpan span)
    {
        Requires.That(span.Length == 0, nameof(span), "span must be empty");
        return Create(span, token: null, DeclaredCapabilitySet.Aliasable);
    }

    public static ICapabilitySetSyntax CreateImplicitAny(TextSpan span)
    {
        Requires.That(span.Length == 0, nameof(span), "span must be empty");
        return Create(span, token: null, DeclaredCapabilitySet.Any);
    }
}
