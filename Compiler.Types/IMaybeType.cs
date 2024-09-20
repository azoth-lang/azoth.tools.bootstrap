using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(IType), typeof(DataType))]
public interface IMaybeType
{
    #region Standard Types
    /// <summary>
    /// The unknown type as <see cref="DataType"/>.
    /// </summary>
    /// <remarks>There are places where the compiler cannot infer the expression type. This can be
    /// used to force the compiler to use <see cref="DataType"/>.</remarks>
    public static readonly DataType Unknown = UnknownType.Instance;
    #endregion
}
