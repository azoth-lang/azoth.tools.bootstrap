using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class CapabilitySetSyntax : CodeSyntax, ICapabilitySetSyntax
{
    public static CapabilitySetSyntax ImplicitAliasable(TextSpan span)
    {
        Requires.That(span.Length == 0, nameof(span), "span must be empty");
        return new(span, CapabilitySet.Aliasable);
    }

    public CapabilitySet Constraint { get; }

    public CapabilitySetSyntax(TextSpan span, CapabilitySet capabilitySet)
        : base(span)
    {
        Constraint = capabilitySet;
    }

    public override string ToString() => Constraint.ToString();
}
