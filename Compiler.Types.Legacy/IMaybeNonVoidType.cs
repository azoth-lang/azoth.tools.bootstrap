using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

[Closed(typeof(INonVoidType), typeof(IMaybeFunctionType))]
public interface IMaybeNonVoidType : IMaybeType
{
    /// <summary>
    /// The same type except with any mutability removed.
    /// </summary>
    new IMaybeNonVoidType WithoutWrite();
    IMaybeType IMaybeType.WithoutWrite() => WithoutWrite();
}
