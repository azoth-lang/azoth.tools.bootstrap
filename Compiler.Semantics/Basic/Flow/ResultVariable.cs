namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public readonly struct ResultVariable
{
    public static readonly ResultVariable First = new(0);

    private readonly long result;

    private ResultVariable(long resultNumber)
    {
        result = resultNumber;
    }

    public ResultVariable NextResult() => new(result + 1);

    public static implicit operator SharingVariable(ResultVariable variable)
        => new SharingVariable(variable.result);

    public override string ToString() => $"⧼result{result}⧽";
}
