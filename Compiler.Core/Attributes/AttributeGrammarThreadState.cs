using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

internal sealed class AttributeGrammarThreadState : IInheritanceContext
{
    public bool InCircle { get; private set; }
    public bool Changed { get; private set; }
    private ulong iteration;
    private readonly Dictionary<AttributeId, ulong> attributeIterations = new();
    private bool isFinal;

    #region Circular Attributes
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CircleScope EnterCircle()
    {
        iteration = 0;
        InCircle = true;
        return new CircleScope(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ExitCircle()
    {
        InCircle = false;
        attributeIterations.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MarkChanged() => Changed = true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void NextIteration()
    {
        iteration += 1;
        Changed = false;
#if DEBUG
        if (iteration > 10_000)
            throw new InvalidOperationException("Circular attribute iteration limit exceeded.");
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateIterationFor(AttributeId attribute)
        => attributeIterations[attribute] = iteration;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ObservedInCycle(AttributeId attribute)
        => attributeIterations.TryGetValue(attribute, out var i) && i == iteration;

    public readonly struct CircleScope(AttributeGrammarThreadState state) : IDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => state.ExitCircle();
    }
    #endregion

    #region "Final" for Cache Control of Non-Circular Attributes and Better Circular Attribute Caching
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MarkNonFinal() => isFinal = false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DependencyScope DependencyContext()
    {
        var scope = new DependencyScope(this, isFinal);
        isFinal = true;
        return scope;
    }

    public readonly struct DependencyScope : IDisposable
    {
        private readonly bool wasFinal;
        private readonly AttributeGrammarThreadState state;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyScope(AttributeGrammarThreadState state, bool wasFinal)
        {
            this.state = state;
            this.wasFinal = wasFinal;
            state.isFinal = true;
        }

        public bool IsFinal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => state.isFinal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => state.isFinal &= wasFinal;
    }
    #endregion

    #region Cycle Detection for Non-Circular Attributes
#if DEBUG
    private readonly HashSet<AttributeId> inProgressAttributes = new();

    /// <summary>
    /// Track which attributes are currently being computed to detect circular dependencies.
    /// </summary>
    public ComputeScope BeginComputing(AttributeId attribute)
    {
        if (!inProgressAttributes.Add(attribute))
            throw new InvalidOperationException($"Attribute `{attribute.ToTypeString()}` has circular definition but is declared non-circular.");
        return new(this, attribute);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EndComputing(AttributeId attribute) => inProgressAttributes.Remove(attribute);

    public readonly struct ComputeScope(AttributeGrammarThreadState state, AttributeId attribute) : IDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => state.EndComputing(attribute);
    }
#endif
    #endregion
}
