using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

/// <summary>
/// A non-conversion that leaves the type unchanged. This is used as an underlying
/// conversion for other conversions when no further conversion is needed.
/// </summary>
public sealed class IdentityConversion : Conversion
{
    #region Singleton
    public static readonly IdentityConversion Instance = new();

    private IdentityConversion() { }
    #endregion

    public override (DataType, ExpressionSemantics) Apply(DataType type, ExpressionSemantics semantics)
        => (type, semantics);

    public override bool IsChainedTo(Conversion conversion) => false;
}
