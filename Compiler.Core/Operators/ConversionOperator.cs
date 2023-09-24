namespace Azoth.Tools.Bootstrap.Compiler.Core.Operators;

public enum ConversionOperator
{
    /// <summary>
    /// The safe conversion operator `as` that only performs a conversion if it is statically safe.
    /// </summary>
    Safe = 1,
    /// <summary>
    /// The aborting conversion operator `as!` that aborts if the conversion fails.
    /// </summary>
    Aborting,
    /// <summary>
    /// The optional return conversion operator `as?` that returns `none` if the conversion fails.
    /// </summary>
    Optional,
}
