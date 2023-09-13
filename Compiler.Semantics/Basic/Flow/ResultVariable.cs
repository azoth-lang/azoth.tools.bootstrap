namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public readonly struct ResultVariable
{
    public static readonly ResultVariable None = new(-1);

    private readonly long result;

    public ResultVariable(long resultNumber)
    {
        result = resultNumber;
    }

    public ResultVariable NextResult() => new(result + 1);

    public static implicit operator SharingVariable(ResultVariable variable)
        => new SharingVariable(variable.result);
}
