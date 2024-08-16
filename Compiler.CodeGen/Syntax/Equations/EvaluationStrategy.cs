namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public enum EvaluationStrategy
{
    /// <summary>
    /// Computed when the node is constructed.
    /// </summary>
    Eager = 1,

    /// <summary>
    /// Computed on demand and cached.
    /// </summary>
    Lazy,

    /// <summary>
    /// Computed on demand and not cached.
    /// </summary>
    Computed,
}
