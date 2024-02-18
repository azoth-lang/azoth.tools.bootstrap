using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(
    typeof(SimpleType))]
public abstract class ValueType : CapabilityType
{
    private protected ValueType() { }
}
