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
    private IMaybeAntetype? namedAntetype;
    private bool namedAntetypeCached;
    public override IMaybeAntetype NamedAntetype
        => GrammarAttribute.IsCached(in namedAntetypeCached) ? namedAntetype!
            : this.Synthetic(ref namedAntetypeCached, ref namedAntetype,
                TypeExpressionsAntetypesAspect.ViewpointType_NamedAntetype);
    private DataType? namedType;
    private bool namedTypeCached;
    public override DataType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType, TypeExpressionsAspect.SelfViewpointType_NamedType);
    private Pseudotype? namedSelfType;
    private bool namedSelfTypeCached;
    public Pseudotype? NamedSelfType
        => GrammarAttribute.IsCached(in namedSelfTypeCached) ? namedSelfType
            : this.Inherited(ref namedSelfTypeCached, ref namedSelfType, InheritedSelfType);

    public SelfViewpointTypeNode(ISelfViewpointTypeSyntax syntax, ITypeNode referent)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }

    protected override void CollectDiagnostics(DiagnosticsBuilder diagnostics)
    {
        TypeExpressionsAspect.SelfViewpointType_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
