using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ConstructorSelfParameterNode : SelfParameterNode, IConstructorSelfParameterNode
{
    public override IConstructorSelfParameterSyntax Syntax { get; }
    public ICapabilityNode Capability { get; }
    public override IdentifierName? Name => Syntax.Name;
    private ValueAttribute<CapabilityType> bindingType;
    public override CapabilityType BindingType
        => bindingType.TryGetValue(out var value) ? value
            : bindingType.GetValue(this, TypeMemberDeclarationsAspect.ConstructorSelfParameter_BindingType);

    public ConstructorSelfParameterNode(IConstructorSelfParameterSyntax syntax, ICapabilityNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        TypeMemberDeclarationsAspect.ConstructorSelfParameter_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics);
    }
}
