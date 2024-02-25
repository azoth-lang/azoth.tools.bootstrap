using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(CapabilityViewpointType), typeof(SelfViewpointType))]
public abstract class ViewpointType : NonEmptyType
{
    public abstract ICapabilityConstraint Capability { get; }

    public abstract DataType Referent { get; }

    public override bool IsFullyKnown => Referent.IsFullyKnown;

    public override TypeSemantics Semantics => Referent.Semantics;

    private protected ViewpointType() { }
}
