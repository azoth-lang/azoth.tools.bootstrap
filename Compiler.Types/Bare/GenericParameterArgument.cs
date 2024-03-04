using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

public readonly record struct GenericParameterArgument(GenericParameter Parameter, DataType Argument)
{
    public ParameterVariance ParameterVariance => Parameter.ParameterVariance;
    public bool ParameterHasIndependence => Parameter.HasIndependence;
    public TypeVariance TypeVariance => Parameter.TypeVariance;
}
