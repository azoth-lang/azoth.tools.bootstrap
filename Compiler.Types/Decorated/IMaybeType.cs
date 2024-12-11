using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[Closed(typeof(IType), typeof(IMaybeNonVoidType))]
public interface IMaybeType : IEquatable<IMaybeType>
{
    #region Standard Types
    public static readonly IMaybeType Unknown = UnknownType.Instance;
    #endregion

    IMaybePlainType PlainType { get; }

    bool HasIndependentTypeArguments { get; }

    /// <summary>
    /// Convert types for literals (e.g. <c>bool[true]</c>, <c>int[42]</c> etc.) to their
    /// corresponding types.
    /// </summary>
    // TODO this makes literal types special. Perhaps there should be a way to declare other literal types in code
    public IMaybeType ToNonLiteral() => this;

    /// <summary>
    /// How this type would be written in source code.
    /// </summary>
    string ToSourceCodeString();

    /// <summary>
    /// How this type would be written in IL.
    /// </summary>
    string ToILString();
}
