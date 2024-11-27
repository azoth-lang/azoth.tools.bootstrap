using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public partial class TypeOperations
{
    public static IFixedList<ParameterType>? AsKnownFixedList(this IEnumerable<IMaybeParameterType> parameters)
    {
        if (parameters is IFixedList<ParameterType> knownParameters)
            return knownParameters;

        return parameters.ToFixedList().As<ParameterType>();
    }

    public static IExpressionType Known(this IMaybeExpressionType type)
        => type switch
        {
            UnknownType _ => throw new InvalidOperationException("Type is not known"),
            IExpressionType t => t,
            _ => throw ExhaustiveMatch.Failed(type),
        };

    public static IType Known(this IMaybeType type)
        => type switch
        {
            UnknownType _ => throw new InvalidOperationException("Type is not known"),
            IType t => t,
            _ => throw ExhaustiveMatch.Failed(type),
        };
}
