using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SelfViewpointTypeNode : TypeNode, ISelfViewpointTypeNode
{
    public override ISelfViewpointTypeSyntax Syntax { get; }

    public ITypeNode Referent { get; }
    private ValueAttribute<IMaybeAntetype> antetype;
    public override IMaybeAntetype NamedAntetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, TypeExpressionsAntetypesAspect.ViewpointType_NamedAntetype);
    private ValueAttribute<DataType> type;
    public override DataType NamedType
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeExpressionsAspect.SelfViewpointType_NamedType);
    private ValueAttribute<Pseudotype?> selfType;
    public Pseudotype? NamedSelfType
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
