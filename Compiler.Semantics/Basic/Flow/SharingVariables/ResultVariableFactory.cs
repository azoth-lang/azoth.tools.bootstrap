namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

public class ResultVariableFactory
{
    private ResultVariable? current;

    public ResultVariable Create() => current = current?.NextResult() ?? ResultVariable.First;
}
