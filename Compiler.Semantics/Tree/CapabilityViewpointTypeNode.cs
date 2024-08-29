using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class CapabilityViewpointTypeNode : TypeNode, ICapabilityViewpointTypeNode
{
    public override ICapabilityViewpointTypeSyntax Syntax { get; }
    public ICapabilityNode Capability { get; }
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
            : this.Synthetic(ref namedTypeCached, ref namedType, TypeExpressionsAspect.CapabilityViewpointType_NamedType);

    public CapabilityViewpointTypeNode(
        ICapabilityViewpointTypeSyntax syntax,
        ICapabilityNode capability,
        ITypeNode referent)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Referent = Child.Attach(this, referent);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics, bool contributeAttribute = true)
    {
        TypeExpressionsAspect.CapabilityViewpointType_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics, contributeAttribute);
    }
}
