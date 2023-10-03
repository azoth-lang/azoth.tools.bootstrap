using System;
using System.Collections.Concurrent;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

/// <summary>
/// A "variable" representing a temporary result in an expression.
/// </summary>
public sealed class ResultVariable : ISharingVariable
{
    #region Cache
    private static readonly ConcurrentDictionary<ulong, ResultVariable> Cache = new();

    private static ResultVariable Create(ulong number) => Cache.GetOrAdd(number, Factory);

    private static ResultVariable Factory(ulong number) => new(number);
    #endregion

    public static readonly ResultVariable First = Create(0);

    private readonly ulong number;

    public bool IsVariableOrParameter => false;
    public bool RestrictsWrite => false;
    public bool IsTracked => true;
    public bool KeepsSetAlive => true;

    private ResultVariable(ulong number)
    {
        this.number = number;
    }

    public ResultVariable NextResult() => Create(number + 1);

    #region Equality
    public bool Equals(ISharingVariable? other) =>
        other is ResultVariable resultVariable && number == resultVariable.number;

    public override bool Equals(object? obj) => obj is ResultVariable other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(number);
    #endregion

    public override string ToString() => $"⧼result{number}⧽";
}
