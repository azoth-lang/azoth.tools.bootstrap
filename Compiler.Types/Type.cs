using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(NonEmptyType), typeof(EmptyType))]
public abstract class Type : DataType, IExpressionType
{
    public override Type AccessedVia(ICapabilityConstraint capability) => this;
}
