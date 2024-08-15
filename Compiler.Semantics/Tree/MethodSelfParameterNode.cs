using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MethodSelfParameterNode : SelfParameterNode, IMethodSelfParameterNode
{
    public override IMethodSelfParameterSyntax Syntax { get; }
    public override IdentifierName? Name => Syntax.Name;
    public ICapabilityConstraintNode Capability { get; }
    private Pseudotype? bindingType;
    private bool bindingTypeCached;
    public override Pseudotype BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType, TypeMemberDeclarationsAspect.MethodSelfParameter_BindingType);

    public MethodSelfParameterNode(IMethodSelfParameterSyntax syntax, ICapabilityConstraintNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        TypeMemberDeclarationsAspect.MethodSelfParameter_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
