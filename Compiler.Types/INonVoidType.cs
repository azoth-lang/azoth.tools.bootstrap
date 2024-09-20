using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(NonEmptyType), typeof(NeverType))]
public interface INonVoidType : IType
{
}
