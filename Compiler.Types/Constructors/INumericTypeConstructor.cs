using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(typeof(NumericTypeConstructor), typeof(IntegerLiteralTypeConstructor))]
public interface INumericTypeConstructor : TypeConstructor
{
    // TODO IntegerLiteralTypeConstructor shouldn't really have a plain type
    OrdinaryNamedPlainType PlainType { get; }
}
