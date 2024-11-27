using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

[Closed(typeof(SelfParameterType), typeof(UnknownType))]
public interface IMaybeSelfParameterType
{
    bool IsLent { get; }

    IMaybePseudotype Type { get; }

    string ToILString();

    string ToSourceCodeString();
}
