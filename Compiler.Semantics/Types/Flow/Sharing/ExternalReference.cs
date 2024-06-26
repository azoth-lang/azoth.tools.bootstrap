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
    private static readonly ConcurrentDictionary<BindingValue, ExternalReference> Cache = new();

    private static ExternalReference Create(BindingValue value) => Cache.GetOrAdd(value, Factory);

    private static ExternalReference Factory(BindingValue value) => new(value.Id.Value, value.Index);
    #endregion

    /// <summary>
    /// An external reference to the parameters of a method that are not lent.
    /// </summary>
    public static readonly ExternalReference NonLentParameters = new(0, CapabilityIndex.TopLevel);

    /// <param name="value">The binding value for the parameter to create a lent external reference
    /// for.</param>
    public static ExternalReference CreateLentParameter(BindingValue value)
        => Create(value);

    /// <summary>
    /// A 1-based parameter number. Zero is reserved for <see cref="NonLentParameters"/>.
    /// </summary>
    private readonly ulong number;

    private readonly CapabilityIndex index;

    private ExternalReference(ulong number, CapabilityIndex index)
    {
        this.number = number;
        this.index = index;
    }

    #region Equality
    public bool Equals(IValue? other)
        => ReferenceEquals(this, other)
           || other is ExternalReference externalReference
               && number == externalReference.number
               && index == externalReference.index;

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is ExternalReference other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(number, index);
    #endregion

    public override string ToString() => number == 0 ? "⧼params⧽" : $"⧼lent-param{number}:{index}⧽";
}
