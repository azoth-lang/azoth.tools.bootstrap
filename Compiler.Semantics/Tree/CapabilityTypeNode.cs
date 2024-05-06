using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class CapabilityTypeNode : TypeNode, ICapabilityTypeNode
{
    public override ICapabilityTypeSyntax Syntax { get; }
    public ICapabilityNode Capability { get; }
    public ITypeNode Referent { get; }

    public CapabilityTypeNode(ICapabilityTypeSyntax syntax, ICapabilityNode capability, ITypeNode referent)
    {
        Syntax = syntax;
        Capability = capability;
        Referent = referent;
    }
}
