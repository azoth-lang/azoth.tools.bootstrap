using System;
using System.Collections.Concurrent;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

/// <summary>
/// Represents the possibility that there are external references to parameters of a method.
/// </summary>
public sealed class ExternalReference : ISharingVariable
{
    #region Cache
    private static readonly ConcurrentDictionary<uint, ExternalReference> Cache = new();

    private static ExternalReference Create(uint number) => Cache.GetOrAdd(number, Factory);

    private static ExternalReference Factory(uint number) => new(number);
    #endregion

    public static readonly ExternalReference NonParameters = new(0);

    public static ExternalReference CreateLentParameter(uint number)
    {
        if (number == 0)
            throw new ArgumentOutOfRangeException(nameof(number), "Lent parameter numbers must be > 0.");
        return Create(number);
    }

    private readonly ulong number;

    public bool IsVariableOrParameter => false;
    public bool RestrictsWrite => false;
    public bool SharingIsTracked => true;
    public bool KeepsSetAlive => false;

    private ExternalReference(uint number)
    {
        this.number = number;
    }

    #region Equality
    public bool Equals(ISharingVariable? other) =>
        other is ExternalReference externalReference && number == externalReference.number;

    public override bool Equals(object? obj) => obj is ExternalReference other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(number);
    #endregion

    public override string ToString() => number == 0 ? "⧼params⧽" : $"⧼lent-param{number}⧽";
}
