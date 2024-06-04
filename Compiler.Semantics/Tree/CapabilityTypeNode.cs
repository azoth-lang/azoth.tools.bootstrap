using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class CapabilityTypeNode : TypeNode, ICapabilityTypeNode
{
    public override ICapabilityTypeSyntax Syntax { get; }
    public ICapabilityNode Capability { get; }
    public ITypeNode Referent { get; }
    private ValueAttribute<IMaybeAntetype> antetype;
    public override IMaybeAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, TypeExpressionsAntetypesAspect.CapabilityType_Antetype);
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeExpressionsAspect.CapabilityType_Type);

    public CapabilityTypeNode(ICapabilityTypeSyntax syntax, ICapabilityNode capability, ITypeNode referent)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Referent = Child.Attach(this, referent);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeExpressionsAspect.CapabilityType_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
