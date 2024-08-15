using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class CapabilityNode : CapabilityConstraintNode, ICapabilityNode
{
    public override ICapabilitySyntax Syntax { get; }
    public Capability Capability => Syntax.Capability;
    public override Capability Constraint => Capability;

    public CapabilityNode(ICapabilitySyntax syntax)
    {
        Syntax = syntax;
    }
}
