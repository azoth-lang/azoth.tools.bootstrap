using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

[Closed(typeof(ParameterType), typeof(SelfParameterType))]
public interface IParameterType
{
    Pseudotype Type { get; }

    string ToILString();
}
