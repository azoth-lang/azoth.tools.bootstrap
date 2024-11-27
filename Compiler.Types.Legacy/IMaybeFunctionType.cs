using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(FunctionType), typeof(UnknownType))]
public interface IMaybeFunctionType : IMaybeNonVoidType
{
    #region Standard Types
    /// <summary>
    /// The unknown type as <see cref="IMaybeFunctionType"/>.
    /// </summary>
    /// <remarks>There are places where the compiler cannot infer the expression type. This can be
    /// used to force the compiler to use <see cref="IMaybeType"/>.</remarks>
    public new static readonly IMaybeFunctionType Unknown = UnknownType.Instance;
    #endregion

    IMaybeType Return { get; }
}
