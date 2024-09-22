using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

[Closed(typeof(ParameterType), typeof(UnknownType))]
public interface IMaybeParameterType
{
    bool IsLent { get; }

    IMaybeNonVoidType Type { get; }

    string ToILString();

    string ToSourceCodeString();
}
