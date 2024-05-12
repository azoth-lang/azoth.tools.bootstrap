using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ConstructorSelfParameterNode : SelfParameterNode, IConstructorSelfParameterNode
{
    public override IConstructorSelfParameterSyntax Syntax { get; }
    public ICapabilityNode Capability { get; }
    public override IdentifierName? Name => Syntax.Name;
    public override Pseudotype Type => throw new NotImplementedException();

    public ConstructorSelfParameterNode(IConstructorSelfParameterSyntax syntax, ICapabilityNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }
}
