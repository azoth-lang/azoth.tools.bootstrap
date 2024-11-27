using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;

[Closed(typeof(SelfParameterType), typeof(UnknownType))]
public interface IMaybeSelfParameterType
{
    bool IsLent { get; }

    IMaybePseudotype Type { get; }

    string ToILString();

    string ToSourceCodeString();
}
