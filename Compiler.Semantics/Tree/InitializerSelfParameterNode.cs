using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InitializerSelfParameterNode : SelfParameterNode, IInitializerSelfParameterNode
{
    public override IInitializerSelfParameterSyntax Syntax { get; }
    public ICapabilityNode Capability { get; }
    public override IdentifierName? Name => Syntax.Name;
    private ValueAttribute<CapabilityType> bindingType;
    public override CapabilityType BindingType
        => bindingType.TryGetValue(out var value) ? value
            : bindingType.GetValue(this, TypeMemberDeclarationsAspect.InitializerSelfParameter_BindingType);

    public InitializerSelfParameterNode(IInitializerSelfParameterSyntax syntax, ICapabilityNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }

    internal override AggregateAttributeNodeKind Diagnostics_NodeKind => AggregateAttributeNodeKind.Contributor;

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        TypeMemberDeclarationsAspect.InitializerSelfParameter_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics);
    }
}
