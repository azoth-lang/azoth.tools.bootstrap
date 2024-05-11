using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(NonEmptyType), typeof(EmptyType))]
public abstract class Type : DataType
{
}
