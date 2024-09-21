using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

[Closed(typeof(ParameterType), typeof(SelfParameterType))]
public interface IParameterType
{
    bool IsLent { get; }

    IMaybePseudotype Type { get; }

    string ToILString();
}
