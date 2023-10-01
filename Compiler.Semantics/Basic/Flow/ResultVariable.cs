namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// A "variable" representing a temporary result in an expression.
/// </summary>
public readonly struct ResultVariable
{
    public static readonly ResultVariable First = new(0);

    public readonly long Number;

    private ResultVariable(long number)
    {
        Number = number;
    }

    public ResultVariable NextResult() => new(Number + 1);

    public override string ToString() => $"⧼result{Number}⧽";
}
