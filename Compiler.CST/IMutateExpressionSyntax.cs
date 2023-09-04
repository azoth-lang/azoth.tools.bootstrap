namespace Azoth.Tools.Bootstrap.Compiler.CST;

/// <summary>
/// i.e. `mut exp`. A mutate expression allows use of a variable to mutate the value. That
/// is, the result is writable reference type. Note that mutate expressions
/// don't apply to value types since they are passed by move, copy, or reference.
/// </summary>
public partial interface IMutateExpressionSyntax
{
}
