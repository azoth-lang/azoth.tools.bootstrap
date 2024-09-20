using System;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public partial class TypeOperations
{
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
