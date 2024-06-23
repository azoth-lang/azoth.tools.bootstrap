using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <remarks><para>The algorithm is based off of Tarjan's algorithm for finding strongly-connected
/// components (SSC) in a graph. Here, the graph is the dependency graph between the cyclic
/// attributes. One difference is that the low-link is not initialized to the index of the current
/// node. Instead, it starts <see langword="null"/> and that is used to detect non-cyclic branches
/// off of any SSCs.</para>
///
/// <para>When an SSC is found, it is iterated on until no changes happen when evaluating. Because
/// the dependency graph can change between iterations, all the nodes of the SSC are popped except
/// for the root. Once there are no changes when evaluating the SSC, all the nodes are popped and
/// marked as cached.</para></remarks>
internal sealed class AttributeGrammarThreadState : IInheritanceContext
{
    public bool InGraph => inStackIndexes.Count > 0;
    public bool Changed { get; private set; }
    /// <summary>
    /// Used to track iterations of circular attribute computation. However, an iteration may only
    /// cover a subset of the attributes in the graph, so it is not a true iteration count.
    /// </summary>
    private ulong currentIteration;
    private ulong nextIndex;
    /// <summary>
    /// The <see cref="currentIteration"/> that each stored non-circular attribute value was last computed.
    /// </summary>
    private readonly Dictionary<AttributeId, ulong> attributeIterations = new();
    private readonly Stack<AttributeCachedRef> outstandingAttributes = new();
    /// <summary>
    /// The indexes assigned to all cyclic attributes on the <see cref="outstandingAttributes"/> stack.
    /// </summary>
    private readonly Dictionary<AttributeId, ulong> inStackIndexes = new();
    private readonly Stack<AttributeId> rewritableAttributes = new();
    private ulong? lowLink;

    #region Cyclic Attributes
    public CyclicScope VisitCyclic(AttributeId attribute, bool isRewritableAttribute, ref bool cached)
    {
#if DEBUG
        // If this is the first attribute, the state ought to already be empty.
        if (!InGraph)
        {
            if (currentIteration != 0)
                throw new InvalidOperationException($"{nameof(currentIteration)} was not zero when entering graph.");
            if (nextIndex != 0)
                throw new InvalidOperationException($"{nameof(nextIndex)} was not zero when entering graph.");
            if (attributeIterations.Any())
                throw new InvalidOperationException($"{nameof(attributeIterations)} was not empty when entering graph.");
            if (outstandingAttributes.Any())
                throw new InvalidOperationException($"{nameof(outstandingAttributes)} was not empty when entering graph.");
            if (rewritableAttributes.Any())
                throw new InvalidOperationException($"{nameof(rewritableAttributes)} was not empty when entering graph.");
        }
#endif

        var index = nextIndex;
        inStackIndexes.Add(attribute, index);
        outstandingAttributes.Push(new(attribute, ref cached));
        if (isRewritableAttribute)
            rewritableAttributes.Push(attribute);
        nextIndex += 1;
        return new(this, attribute, isRewritableAttribute, index);
    }

    public readonly struct CyclicScope : IDisposable
    {
        private readonly AttributeGrammarThreadState state;
        private readonly AttributeId attribute;
        private readonly ulong attributeIndex;
        private readonly bool wasChanged;
        private readonly bool isRewritableAttribute;
        private readonly ulong? previousLowLink;

        internal CyclicScope(
            AttributeGrammarThreadState state,
            AttributeId attribute,
            bool isRewritableAttribute,
            ulong attributeIndex)
        {
            this.state = state;
            this.attribute = attribute;
            this.isRewritableAttribute = isRewritableAttribute;
            this.attributeIndex = attributeIndex;
            wasChanged = state.Changed;
            previousLowLink = state.lowLink;
        }

        public bool RootOfChangedComponent => state.Changed && IsRootOfComponent;

        /// <summary>
        /// Whether this attribute is the root of a strongly-connected component in the attribute
        /// dependency graph.
        /// </summary>
        private bool IsRootOfComponent => state.lowLink == attributeIndex;

        /// <summary>
        /// Whether the value of the attribute is final and should be cached.
        /// </summary>
        /// <remarks>If <see cref="lowLink"/> is <see langword="null"/> then there were no cycles in
        /// the subgraph and this attribute is final. Alternatively, if this is the root of a
        /// strongly-connected component that has no changes it in, then  the component is final. It
        /// isn't possible to mark the whole component final, but the root is marked final so that
        /// future visits will propagate final values outward.</remarks>
        public bool IsFinal
            => state.lowLink is null || (!state.Changed && IsRootOfComponent);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NextIteration()
        {
            // Remove attributes off the stack up to the current attribute. We are reevaluating the
            // current attribute.
            while (state.outstandingAttributes.Peek().Attribute != attribute)
            {
                var popped = state.outstandingAttributes.Pop();
                if (!state.inStackIndexes.Remove(popped.Attribute))
                    throw new InvalidOperationException("Attribute was not on stack.");
            }

            // Reset the next index since we popped off all those attributes.
            state.nextIndex = attributeIndex + 1;

            state.currentIteration += 1;
#if DEBUG
            if (state.currentIteration > 10_000)
                throw new InvalidOperationException("Circular attribute iteration limit exceeded.");
#endif
            state.Changed = false;
            state.lowLink = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkChanged() => state.Changed = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkFinal() => state.lowLink = null;

        public void Dispose()
        {
            if (IsFinal)
            {
                if (IsRootOfComponent)
                {
                    // If there were changes, then this attribute was marked final by another thread,
                    // and we shouldn't mark any attributes in the SCC as final.
                    var markFinal = !state.Changed;
                    // Remove attributes off the stack up to and including the current attribute.
                    AttributeCachedRef popped;
                    while ((popped = state.outstandingAttributes.Pop()).Attribute != attribute)
                    {
                        if (!state.inStackIndexes.Remove(popped.Attribute))
                            throw new InvalidOperationException("Attribute was not on stack.");

                        if (markFinal)
                            Volatile.Write(ref popped.Cached, true);
                    }
                }
                else if (state.outstandingAttributes.Pop().Attribute != attribute)
                    throw new InvalidOperationException("Non-root attribute not on top of stack");

                if (!state.inStackIndexes.Remove(attribute))
                    throw new InvalidOperationException("Attribute was not on stack.");

                state.nextIndex = attributeIndex;
                state.Changed = wasChanged;
                state.lowLink = previousLowLink;
                if (attributeIndex == 0)
                {
                    state.currentIteration = 0;
                    state.attributeIterations.Clear();
                }
            }
            else
            {
                if (attributeIndex == 0)
                    throw new InvalidOperationException("The starting attribute should always end up final.");

                state.Changed |= wasChanged;

                if (previousLowLink is ulong previous)
                    state.lowLink = Math.Min(previous, state.lowLink ?? previous);
            }

            if (isRewritableAttribute)
                state.rewritableAttributes.Pop();
        }
    }

    public bool CheckInStackAndUpdateLowLink(AttributeId attribute)
    {
        if (!inStackIndexes.TryGetValue(attribute, out var index))
            return false;

        lowLink = Math.Min(lowLink ?? index, index);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ComputedInCurrentIteration(AttributeId attribute)
        => attributeIterations.TryGetValue(attribute, out var i) && i == currentIteration;
    #endregion

    #region Non-Circular Attributes
#if DEBUG
    private readonly HashSet<AttributeId> inProgressAttributes = new();
#endif
    /// <summary>
    /// Track which attributes are currently being computed to detect circular dependencies.
    /// </summary>
    public NonCircularScope VisitNonCircular(AttributeId attribute)
    {
#if DEBUG
        if (!InGraph)
            if (!inProgressAttributes.Add(attribute))
                throw new InvalidOperationException($"Attribute `{attribute.ToTypeString()}` has circular definition but is declared non-circular.");
#endif
        return new(this, attribute);
    }

    public readonly struct NonCircularScope(AttributeGrammarThreadState state, AttributeId attribute) : IDisposable
    {
        public bool IsFinal => state.lowLink is null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkComputedInCurrentIteration(AttributeId attribute)
        {
            // If current iteration is zero, we are outside a cyclic attribute computation.
            if (state.currentIteration != 0)
                state.attributeIterations[attribute] = state.currentIteration;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComputedInIteration(AttributeId attributeId)
        {
            // If current iteration is zero, we are outside a cyclic attribute computation.
            if (state.currentIteration != 0)
                state.attributeIterations.Remove(attributeId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Style", "IDE0022:Use expression body for method",
            Justification = "<Pending>")]
        public void Dispose()
        {
#if DEBUG
            state.inProgressAttributes.Remove(attribute);
#endif
        }
    }
    #endregion
}
