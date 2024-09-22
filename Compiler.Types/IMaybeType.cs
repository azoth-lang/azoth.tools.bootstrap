using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(IType), typeof(IMaybeNonVoidType))]
public interface IMaybeType : IMaybeExpressionType
{
    #region Standard Types
    /// <summary>
    /// The unknown type as <see cref="IMaybeType"/>.
    /// </summary>
    /// <remarks>There are places where the compiler cannot infer the expression type. This can be
    /// used to force the compiler to use <see cref="IMaybeType"/>.</remarks>
    public new static readonly IMaybeType Unknown = UnknownType.Instance;
    #endregion

    public new IMaybeAntetype ToAntetype();
    IMaybeExpressionAntetype IMaybePseudotype.ToAntetype() => ToAntetype();
}
