using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

/// <summary>
/// A generic parameter definition for an antetype.
/// </summary>
public sealed class AntetypeGenericParameter
{
    public IdentifierName Name { get; }

    public ParameterVariance Variance { get; }

    public AntetypeGenericParameter(
        IdentifierName name,
        ParameterVariance variance)
    {
        Variance = variance;
        Name = name;
    }
}
