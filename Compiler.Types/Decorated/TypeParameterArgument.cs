using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public readonly record struct TypeParameterArgument(TypeConstructor.Parameter Parameter, Type Argument)
{
    public bool ParameterHasIndependence => Parameter.HasIndependence;
    public TypeParameterIndependence Independence => Parameter.Independence;
    public TypeParameterVariance Variance => Parameter.Variance;
}
