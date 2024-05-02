using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class CapabilitySetNode : CapabilityConstraintNode, ICapabilitySetNode
{
    public override ICapabilitySetSyntax Syntax { get; }
    public override CapabilitySet Constraint => Syntax.Constraint;

    public CapabilitySetNode(ICapabilitySetSyntax syntax)
    {
        Syntax = syntax;
    }
}
