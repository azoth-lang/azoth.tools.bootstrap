using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;

[Closed(typeof(ParameterType), typeof(UnknownType))]
public interface IMaybeParameterType
{
    bool IsLent { get; }

    IMaybeNonVoidType Type { get; }

    IMaybeNonVoidPlainType ToPlainType();

    string ToILString();

    string ToSourceCodeString();
}
