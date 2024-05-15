using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class CapabilityViewpointTypeNode : TypeNode, ICapabilityViewpointTypeNode
{
    public override ICapabilityViewpointTypeSyntax Syntax { get; }
    public ICapabilityNode Capability { get; }
    public ITypeNode Referent { get; }
    private ValueAttribute<DataType> type;
    public override DataType Type
    => type.TryGetValue(out var value) ? value
        : type.GetValue(this, TypeExpressionsAspect.CapabilityViewpointType_Type);

    public CapabilityViewpointTypeNode(
        ICapabilityViewpointTypeSyntax syntax,
        ICapabilityNode capability,
        ITypeNode referent)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Referent = Child.Attach(this, referent);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeExpressionsAspect.CapabilityViewpointType_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
