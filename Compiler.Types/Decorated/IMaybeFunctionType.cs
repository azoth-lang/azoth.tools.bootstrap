using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[Closed(typeof(FunctionType), typeof(UnknownType))]
public interface IMaybeFunctionType : IMaybeNonVoidType
{
    #region Standard Types
    public static new readonly IMaybeFunctionType Unknown = UnknownType.Instance;
    #endregion

    new IMaybeFunctionPlainType PlainType { get; }
    IMaybeNonVoidPlainType IMaybeNonVoidType.PlainType => PlainType;

    IMaybeType Return { get; }
}
