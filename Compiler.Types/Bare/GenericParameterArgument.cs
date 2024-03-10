using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

public readonly record struct GenericParameterArgument(GenericParameter Parameter, DataType Argument)
{
    public bool ParameterHasIndependence => Parameter.HasIndependence;
    public ParameterIndependence Independence => Parameter.Independence;
    public TypeVariance Variance => Parameter.Variance;
}
