using System.Runtime.InteropServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// TODO rename to GenericArgument?
[StructLayout(LayoutKind.Auto)]
public readonly record struct TypeParameterArgument(TypeConstructorParameter Parameter, Type Argument)
{
    public bool ParameterHasIndependence => Parameter.HasIndependence;
    public TypeParameterIndependence Independence => Parameter.Independence;
    public TypeParameterVariance Variance => Parameter.Variance;
}
