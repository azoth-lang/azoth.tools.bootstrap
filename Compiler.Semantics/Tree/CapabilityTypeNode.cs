using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class CapabilityTypeNode : TypeNode, ICapabilityTypeNode
{
    public override ICapabilityTypeSyntax Syntax { get; }
    public ICapabilityNode Capability { get; }
    public ITypeNode Referent { get; }
    private IMaybeAntetype? namedAntetype;
    private bool namedAntetypeCached;
    public override IMaybeAntetype NamedAntetype
        => GrammarAttribute.IsCached(in namedAntetypeCached) ? namedAntetype!
            : this.Synthetic(ref namedAntetypeCached, ref namedAntetype,
                TypeExpressionsAntetypesAspect.CapabilityType_NamedAntetype);
    private DataType? namedType;
    private bool namedTypeCached;
    public override DataType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType, TypeExpressionsAspect.CapabilityType_NamedType);

    public CapabilityTypeNode(ICapabilityTypeSyntax syntax, ICapabilityNode capability, ITypeNode referent)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Referent = Child.Attach(this, referent);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        TypeExpressionsAspect.CapabilityType_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics);
    }
}
