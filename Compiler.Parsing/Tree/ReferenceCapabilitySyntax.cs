using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ReferenceCapabilitySyntax : Syntax, IReferenceCapabilitySyntax
{
    public static ReferenceCapabilitySyntax ImplicitReadOnly(TextSpan span)
    {
        Requires.That(nameof(span), span.Length == 0, "span must be empty");
        return new(span, Enumerable.Empty<ICapabilityToken>(), DeclaredReferenceCapability.ReadOnly);
    }

    public FixedList<ICapabilityToken> Tokens { get; }
    public DeclaredReferenceCapability Declared { get; }

    public ReferenceCapabilitySyntax(
        TextSpan span,
        IEnumerable<ICapabilityToken> tokens,
        DeclaredReferenceCapability declared)
        : base(span)
    {
        Tokens = tokens.ToFixedList();
        Declared = declared;
    }

    public override string ToString()
    {
        return string.Join(' ', Tokens);
    }
}
