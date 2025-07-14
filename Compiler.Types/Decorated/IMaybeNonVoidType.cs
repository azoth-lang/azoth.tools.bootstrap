using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[Closed(typeof(NonVoidType), typeof(IMaybeFunctionType))]
public interface IMaybeNonVoidType : IMaybeType
{
    new IMaybeNonVoidPlainType PlainType { get; }
    IMaybePlainType IMaybeType.PlainType => PlainType;

    IMaybeNonVoidType? BaseType { get; }

    /// <summary>
    /// Convert types for literals (e.g. <c>bool[true]</c>, <c>int[42]</c> etc.) to their
    /// corresponding types.
    /// </summary>
    // TODO this makes literal types special. Perhaps there should be a way to declare other literal types in code
    new IMaybeNonVoidType ToNonLiteral();
    IMaybeType IMaybeType.ToNonLiteral() => ToNonLiteral();
}
