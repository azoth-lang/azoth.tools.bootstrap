using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

/// <summary>
/// Represents the possibility that there are external references to parameters of a method.
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
internal sealed class ExternalReference : IValue
{
    #region Cache
    private static readonly ConcurrentDictionary<ulong, ExternalReference> Cache = new();

    private static ExternalReference Create(ulong number) => Cache.GetOrAdd(number, Factory);

    private static ExternalReference Factory(ulong number) => new(number);
    #endregion

    /// <summary>
    /// An external reference to the parameters of a method that are not lent.
    /// </summary>
    public static readonly ExternalReference NonLentParameters = new(0);

    /// <param name="parameterIndex">The zero based index of the parameter this is being created
    /// for. If this invocable has a <c>self</c> parameter, then that is parameter 0.</param>
    /// <returns></returns>
    public static ExternalReference CreateLentParameter(ulong parameterIndex)
        => Create(parameterIndex + 1);

    /// <summary>
    /// A 1-based parameter number. Zero is reserved for <see cref="NonLentParameters"/>.
    /// </summary>
    private readonly ulong number;

    public bool IsVariableOrParameter => false;
    public CapabilityRestrictions RestrictionsImposed => CapabilityRestrictions.None;
    public bool SharingIsTracked => true;
    public bool KeepsSetAlive => false;

    private ExternalReference(ulong number)
    {
        this.number = number;
    }

    #region Equality
    public bool Equals(IValue? other)
        => ReferenceEquals(this, other)
           || other is ExternalReference externalReference && number == externalReference.number;

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is ExternalReference other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(number);
    #endregion

    public override string ToString() => number == 0 ? "⧼params⧽" : $"⧼lent-param{number}⧽";
}
