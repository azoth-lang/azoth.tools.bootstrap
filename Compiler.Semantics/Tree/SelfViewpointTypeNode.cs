using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SelfViewpointTypeNode : TypeNode, ISelfViewpointTypeNode
{
    public override ISelfViewpointTypeSyntax Syntax { get; }

    public ITypeNode Referent { get; }
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeExpressionsAspect.SelfViewpointType_Type);
    private ValueAttribute<Pseudotype?> selfType;
    public Pseudotype? SelfType
        => selfType.TryGetValue(out var value) ? value
            : selfType.GetValue(InheritedSelfType);

    public SelfViewpointTypeNode(ISelfViewpointTypeSyntax syntax, ITypeNode referent)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeExpressionsAspect.SelfViewpointType_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
