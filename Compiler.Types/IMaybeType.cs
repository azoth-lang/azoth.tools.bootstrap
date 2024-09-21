using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(IType), typeof(IMaybeNonVoidType))]
public interface IMaybeType : IMaybeExpressionType
{
    #region Standard Types
    /// <summary>
    /// The unknown type as <see cref="DataType"/>.
    /// </summary>
    /// <remarks>There are places where the compiler cannot infer the expression type. This can be
    /// used to force the compiler to use <see cref="DataType"/>.</remarks>
    public static readonly IMaybeExpressionType Unknown = UnknownType.Instance;
    #endregion

    public new IMaybeAntetype ToAntetype();
    IMaybeExpressionAntetype IMaybePseudotype.ToAntetype() => ToAntetype();
}
