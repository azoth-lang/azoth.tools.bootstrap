using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

// TODO change Argument to IType
public readonly record struct GenericParameterArgument(GenericParameter Parameter, IExpressionType Argument)
{
    public bool ParameterHasIndependence => Parameter.HasIndependence;
    public TypeParameterIndependence Independence => Parameter.Independence;
    public TypeParameterVariance Variance => Parameter.Variance;
}
