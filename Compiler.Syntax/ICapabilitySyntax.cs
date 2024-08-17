using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface ICapabilitySyntax
{
    public static ICapabilitySyntax Create(
        TextSpan span,
        IEnumerable<ICapabilityToken> tokens,
        DeclaredCapability declared)
    {
        // TODO allow AG to specify the capability
        var capability = declared.ToCapability();
        return Create(span, capability, tokens.ToFixedList(), declared, capability);
    }

    public static ICapabilitySyntax CreateImplicitReadOnly(TextSpan span)
    {
        Requires.That(span.Length == 0, nameof(span), "span must be empty");
        return Create(span, [], DeclaredCapability.Read);
    }
}
