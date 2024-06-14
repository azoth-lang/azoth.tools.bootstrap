using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

internal sealed class AttributeGrammarThreadState
{
    public bool InCircle { get; private set; }
    public bool Changed { get; private set; }
    private ulong iteration;
    private readonly Dictionary<AttributeId, ulong> attributeIterations = new();


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
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateIterationFor(AttributeId attribute)
        => attributeIterations[attribute] = iteration;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ObservedInCycle(AttributeId attribute)
        => attributeIterations.TryGetValue(attribute, out var i) && i != iteration;

    public readonly struct CircleScope(AttributeGrammarThreadState state) : IDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => state.ExitCircle();
    }

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
}
