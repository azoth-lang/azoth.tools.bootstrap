using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(typeof(FunctionAntetype), typeof(UnknownAntetype))]
public interface IMaybeFunctionAntetype : IMaybeNonVoidAntetype;
