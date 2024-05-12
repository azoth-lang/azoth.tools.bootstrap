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
    private ValueAttribute<Pseudotype> type;
    public override Pseudotype Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, InvocableDeclarationsAspect.MethodSelfParameter_Type);
    private ValueAttribute<SelfParameter> parameterType;
    public SelfParameter ParameterType
        => parameterType.TryGetValue(out var value) ? value
            : parameterType.GetValue(this, InvocableDeclarationsAspect.MethodSelfParameter_ParameterType);

    public MethodSelfParameterNode(IMethodSelfParameterSyntax syntax, ICapabilityConstraintNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        InvocableDeclarationsAspect.MethodSelfParameter_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
