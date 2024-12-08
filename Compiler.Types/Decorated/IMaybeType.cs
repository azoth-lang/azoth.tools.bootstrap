using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[Closed(typeof(IType), typeof(UnknownType))]
public interface IMaybeType : IEquatable<IMaybeType>
{
    IMaybePlainType PlainType { get; }

    /// <summary>
    /// How this type would be written in source code.
    /// </summary>
    string ToSourceCodeString();

    /// <summary>
    /// How this type would be written in IL.
    /// </summary>
    string ToILString();
}
