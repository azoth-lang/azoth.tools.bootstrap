using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MethodSelfParameterNode : SelfParameterNode, IMethodSelfParameterNode
{
    public override IMethodSelfParameterSyntax Syntax { get; }
    public override IdentifierName? Name => Syntax.Name;
    public ICapabilityConstraintNode Capability { get; }
    private ValueAttribute<Pseudotype> bindingType;
    public override Pseudotype BindingType
        => bindingType.TryGetValue(out var value) ? value
            : bindingType.GetValue(this, TypeMemberDeclarationsAspect.MethodSelfParameter_BindingType);
    private ValueAttribute<SelfParameterType> parameterType;
    public SelfParameterType ParameterType
        => parameterType.TryGetValue(out var value) ? value
            : parameterType.GetValue(this, TypeMemberDeclarationsAspect.MethodSelfParameter_ParameterType);

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
