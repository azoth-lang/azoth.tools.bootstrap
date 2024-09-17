using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(CapabilityViewpointType), typeof(SelfViewpointType))]
public abstract class ViewpointType : NonEmptyType
{
    public abstract ICapabilityConstraint Capability { get; }

    public abstract Type Referent { get; }

    public override bool IsFullyKnown => Referent.IsFullyKnown;

    private protected ViewpointType() { }

    public sealed override IMaybeExpressionAntetype ToAntetype() => Referent.ToAntetype();
}
