using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class CapabilitySetSyntax : Syntax, ICapabilitySetSyntax
{
    public static CapabilitySetSyntax ImplicitAliasable(TextSpan span)
    {
        Requires.That(nameof(span), span.Length == 0, "span must be empty");
        return new(span, CapabilitySet.Aliasable);
    }

    public CapabilitySet Constraint { get; }
    ICapabilityConstraint ICapabilityConstraintSyntax.Constraint => Constraint;

    public CapabilitySetSyntax(TextSpan span, CapabilitySet capabilitySet)
        : base(span)
    {
        Constraint = capabilitySet;
    }

    public override string ToString() => Constraint.ToString();
}
