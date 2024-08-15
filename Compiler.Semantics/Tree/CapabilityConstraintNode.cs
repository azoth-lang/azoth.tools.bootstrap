using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class CapabilityConstraintNode : CodeNode, ICapabilityConstraintNode
{
    public abstract override ICapabilityConstraintSyntax Syntax { get; }
    public abstract ICapabilityConstraint Constraint { get; }
}
