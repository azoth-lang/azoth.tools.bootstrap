using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public partial class TypeOperations
{
    public static IFixedList<ParameterType>? AsKnownFixedList(this IEnumerable<IMaybeParameterType> parameters)
    {
        if (parameters is IFixedList<ParameterType> knownParameters)
            return knownParameters;

        return parameters.ToFixedList().As<ParameterType>();
    }

    public static Type Known(this IMaybeType type)
        => type switch
        {
            UnknownType _ => throw new InvalidOperationException("Type is not known"),
            Type t => t,
            _ => throw ExhaustiveMatch.Failed(type),
        };
}
