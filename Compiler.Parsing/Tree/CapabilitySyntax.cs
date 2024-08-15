using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class CapabilitySyntax : CodeSyntax, ICapabilitySyntax
{
    public static CapabilitySyntax ImplicitReadOnly(TextSpan span)
    {
        Requires.That(span.Length == 0, nameof(span), "span must be empty");
        return new(span, Enumerable.Empty<ICapabilityToken>(), DeclaredCapability.Read);
    }

    public IFixedList<ICapabilityToken> Tokens { get; }
    public DeclaredCapability Declared { get; }
    public Capability Capability { get; }
    ICapabilityConstraint ICapabilityConstraintSyntax.Constraint => Capability;

    public CapabilitySyntax(
        TextSpan span,
        IEnumerable<ICapabilityToken> tokens,
        DeclaredCapability declared)
        : base(span)
    {
        Tokens = tokens.ToFixedList();
        Declared = declared;
        Capability = Declared.ToCapability();
    }

    public override string ToString() => Declared.ToCapability().ToSourceCodeString();
}
