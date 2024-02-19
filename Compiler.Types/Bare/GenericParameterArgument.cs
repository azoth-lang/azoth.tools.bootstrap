using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

public readonly record struct GenericParameterArgument(GenericParameter Parameter, DataType Argument);
