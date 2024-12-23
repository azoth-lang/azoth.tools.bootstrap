namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

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

public static class EvaluationStrategyExtensions
{
    public static string ToSourceCodeString(this EvaluationStrategy? strategy)
        => strategy is { } s ? s.ToSourceCodeString() : "";

    public static string ToSourceCodeString(this EvaluationStrategy strategy)
        => strategy switch
        {
            EvaluationStrategy.Eager => "eager",
            EvaluationStrategy.Lazy => "lazy",
            EvaluationStrategy.Computed => "computed",
            _ => throw new($"Unknown evaluation strategy: {strategy}"),
        };

    public static EvaluationStrategy WithDefault(
        this EvaluationStrategy? strategy,
        string? expression,
        EvaluationStrategy defaultStrategy = EvaluationStrategy.Lazy)
        => strategy
           ?? (expression is not null ? EvaluationStrategy.Computed : defaultStrategy);
}
