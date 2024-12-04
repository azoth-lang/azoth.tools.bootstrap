using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public interface IMaybeType
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
