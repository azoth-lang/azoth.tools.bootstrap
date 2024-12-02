using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

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

    public new IMaybePlainType ToPlainType();
    IMaybePlainType IMaybePseudotype.ToPlainType() => ToPlainType();

    /// <summary>
    /// The same type except with any mutability removed.
    /// </summary>
    IMaybeType WithoutWrite();

    /// <summary>
    /// Return the type for when a value of this type is accessed via a type of the given value.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    new IMaybeType AccessedVia(IMaybePseudotype contextType);
    IMaybeExpressionType IMaybeExpressionType.AccessedVia(IMaybePseudotype contextType)
        => AccessedVia(contextType);

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    new IMaybeType AccessedVia(ICapabilityConstraint capability);
    IMaybeExpressionType IMaybeExpressionType.AccessedVia(ICapabilityConstraint capability)
        => AccessedVia(capability);
}
