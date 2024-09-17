using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Framework;
using InlineMethod;

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
    private readonly RewriteContexts rewriteContexts = new();
    private ulong? lowLink;

    void IInheritanceContext.AccessParentOf(IChildTreeNode childNode)
        // If the child node is part of a rewrite context, then accessing the parent means this is
        // part of the SSC of the rewrite context. This is true because a rewrite can insert a new
        // node between the parent and child. Thus, the parent of even the top-most rewrite node
        // could change.
        => MinLowLinkWith(rewriteContexts.LowLinkFor(childNode));

    [Inline]
    private void MinLowLinkWith(ulong? value)
    {
        if (value is null)
            return;
        lowLink = lowLink is null ? value : Math.Min(lowLink.Value, value.Value);
    }

    #region Cyclic Attributes
    public bool CheckInStackAndUpdateLowLink(in AttributeId attribute)
    {
        if (!inStackIndexes.TryGetValue(attribute, out var index))
            return false;

        MinLowLinkWith(index);
        return true;
    }

    public CyclicScope VisitCyclic(
        in AttributeId attribute,
        InteriorRef<bool> cached,
        bool isRewritableAttribute,
        IChildTreeNode? child)
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
            if (lowLink is not null)
                throw new InvalidOperationException($"{nameof(lowLink)} was not null when entering graph..");
        }
#endif

        var index = nextIndex;
        nextIndex += 1;
        outstandingAttributes.Push(new(attribute, cached));
        var rewriteContext = isRewritableAttribute
            ? rewriteContexts.NewRewrite(child!, index) : null;
        return new(this, attribute, rewriteContext, index);
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly ref struct CyclicScope
    {
        private readonly AttributeGrammarThreadState state;
        private readonly AttributeId attribute;
        private readonly ulong attributeIndex;
        private readonly bool wasChanged;
        private readonly RewriteContext? rewriteContext;
        private readonly ulong? previousLowLink;

        internal CyclicScope(
            AttributeGrammarThreadState state,
            in AttributeId attribute,
            RewriteContext? rewriteContext,
            ulong attributeIndex)
        {
            this.state = state;
            this.attribute = attribute;
            this.attributeIndex = attributeIndex;
            this.rewriteContext = rewriteContext;
            wasChanged = state.Changed;
            previousLowLink = state.lowLink;
            state.inStackIndexes.Add(attribute, attributeIndex);
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
        public void RewritableDependsOnSelf() => state.MinLowLinkWith(attributeIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkChanged() => state.Changed = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkFinal() => state.lowLink = null;

        /// <summary>
        /// Mark that this attribute is final because it cannot be rewritten further. It still may
        /// be the case that it is part of a cycle that needs to be reevaluated.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkRewritableFinal()
        {
            // If the low link is below the current attribute, then there may be a cycle below this
            // attribute, but it doesn't matter. On the other hand, if the cycle links to a node
            // above this one, then it could have seen this node in a previous state and need
            // reevaluated. Do not set lowLink to null because that would be MarkFinal() and cause
            // all attributes below this one to be marked final. That may not be correct. All we
            // know is that this node is final. To handle this, we mark this node as having not
            // changed. From the perspective of the caller, it didn't change, the final value was
            // returned on the first request and no previous value was observed.
            if (state.lowLink >= attributeIndex)
                state.Changed = false;
        }

        public void AddToRewriteContext(object current, object? next)
            => state.rewriteContexts.AddRewriteToContext((IChildTreeNode)current, (IChildTreeNode?)next);

        /// <summary>
        /// Must be called before successfully leaving the attribute scope.
        /// </summary>
        /// <remarks>This performs safety checks. These cannot be done in the <see cref="Dispose"/>
        /// method because if it fails, an exception is thrown and that exception causes any original
        /// exception to be lost. This method will typically do that. Indeed, an actual error was
        /// observed where that happened.</remarks>
        [Conditional("DEBUG")]
        public void Success()
        {
            if (attributeIndex == 0 && !IsFinal)
                throw new InvalidOperationException("The starting attribute should always end up final.");
        }

        public void Dispose()
        {
            rewriteContext?.MarkInactive();

            if (IsFinal)
            {
                // If there were changes, then this attribute was marked final by another thread OR
                // is final because it doesn't support anymore rewrites. So shouldn't mark any
                // attributes below it as cached.
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

                if (!state.inStackIndexes.Remove(attribute))
                    throw new InvalidOperationException("Attribute was not on stack.");

                state.nextIndex = attributeIndex;
                state.Changed = wasChanged;
                state.lowLink = previousLowLink;
            }
            else
            {
                state.Changed |= wasChanged;

                state.MinLowLinkWith(previousLowLink);
            }

            // If this is the first attribute, reset the state. (Note: this happens regardless of
            // whether the attribute is final or not because in the case of an exception, the
            // attribute won't be final, but cleanup is still needed to prevent other work using this
            // thread state from being affected.)
            if (attributeIndex == 0)
            {
                state.currentIteration = 0;
                state.attributeIterations.Clear();
                state.rewriteContexts.Clear();

                // These should already be correct, but in case of an exception, still reset them.
                state.outstandingAttributes.Clear();
                state.inStackIndexes.Clear(); // Critical because it is used to determine InGraph.
                state.nextIndex = 0;
                if (!state.inAttributesOutsideOfGraph)
                    // lowLink is cleared only if we are not in a non-cyclic attribute computation.
                    // If we are, then the lowLink indicates that those attributes are NOT final. It
                    // is cleared when the non-cyclic attribute computation is done.
                    state.lowLink = null;
            }
        }
    }
    #endregion

    #region Non-Circular Attributes
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ComputedInCurrentIteration(in AttributeId attribute)
        => attributeIterations.TryGetValue(attribute, out var i) && i == currentIteration;

    /// <summary>
    /// Whether we are currently inside the evaluation of attributes outside a graph of cyclic attributes.
    /// </summary>
    private bool inAttributesOutsideOfGraph;
#if DEBUG
    private readonly HashSet<AttributeId> inProgressAttributes = new();
#endif
    /// <summary>
    /// Track which attributes are currently being computed to detect circular dependencies.
    /// </summary>
    public NonCircularScope VisitNonCircular(in AttributeId attribute)
    {
#if DEBUG
        if (!InGraph && !inProgressAttributes.Add(attribute))
            throw new InvalidOperationException($"Attribute `{attribute.ToTypeString()}` has circular definition but is declared non-circular.");
#endif
        return new(this, attribute);
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly ref struct NonCircularScope
    {
        private readonly AttributeGrammarThreadState state;
        private readonly AttributeId attribute;
        private readonly bool firstAttribute;
        private readonly ulong? previousLowLink;

        public NonCircularScope(AttributeGrammarThreadState state, in AttributeId attribute)
        {
            this.state = state;
            this.attribute = attribute;
            previousLowLink = state.lowLink;

            if (state is { inAttributesOutsideOfGraph: false, InGraph: false })
            {
                if (state.lowLink is not null)
                    throw new InvalidOperationException("Low link should be null when starting attribute computation.");
                state.inAttributesOutsideOfGraph = true;
                firstAttribute = true;
            }
            else
                // Clear low link because we are starting a potentially new branch from the graph.
                state.lowLink = null;
        }

        public bool IsFinal => state.lowLink is null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkComputedInCurrentIteration()
        {
            // If current iteration is zero, we are outside a cyclic attribute computation.
            if (state.currentIteration != 0)
                state.attributeIterations[attribute] = state.currentIteration;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComputedInIteration()
        {
            // If current iteration is zero, we are outside a cyclic attribute computation.
            if (state.currentIteration != 0)
                state.attributeIterations.Remove(attribute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (firstAttribute)
            {
                state.inAttributesOutsideOfGraph = false;
                // Reset the low link so that any future thread using this state won't be affected.
                state.lowLink = null;
            }
            else
                // Restore any previous lowLink to not mess up the computation of the cyclic attributes.
                state.MinLowLinkWith(previousLowLink);
#if DEBUG
            state.inProgressAttributes.Remove(attribute);
#endif
        }
    }
    #endregion
}
