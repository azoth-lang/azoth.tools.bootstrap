using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
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
            : GrammarAttribute.Synthetic(ref bindingTypeCached, this,
                TypeMemberDeclarationsAspect.MethodSelfParameter_BindingType, ref bindingType);

    public MethodSelfParameterNode(IMethodSelfParameterSyntax syntax, ICapabilityConstraintNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeMemberDeclarationsAspect.MethodSelfParameter_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
