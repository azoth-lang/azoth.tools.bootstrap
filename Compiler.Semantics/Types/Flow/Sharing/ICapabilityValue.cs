namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

/// <summary>
/// A <see cref="IValue"/> that has its capability affected by flow typing and sharing.
/// </summary>
public interface ICapabilityValue : IValue
{
    ulong Value { get; }
    CapabilityIndex Index { get; }
}
