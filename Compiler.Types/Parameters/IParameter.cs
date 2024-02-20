using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

[Closed(typeof(Parameter), typeof(SelfParameter))]
public interface IParameter
{
    bool IsLent { get; }

    Pseudotype Type { get; }

    string ToILString();
}
