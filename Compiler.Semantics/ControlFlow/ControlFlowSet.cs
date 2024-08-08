using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

public class ControlFlowSet : IReadOnlyCollection<IControlFlowNode>, IReadOnlyDictionary<IControlFlowNode, ControlFlowKind>
{
    public static ControlFlowSet Empty { get; } = new();

    public static ControlFlowSet Create(IReadOnlyDictionary<IControlFlowNode, ControlFlowKind> set)
        => new(new Dictionary<IControlFlowNode, ControlFlowKind>(set));

    public static ControlFlowSet CreateNormal(params IControlFlowNode[] nodes)
        => new(nodes, ControlFlowKind.Normal);

    public static ControlFlowSet CreateNormal(IControlFlowNode? node)
        => node is null ? Empty : new([node], ControlFlowKind.Normal);

    public static ControlFlowSet CreateNormal(IControlFlowNode node1, IControlFlowNode node2)
        => new([node1, node2], ControlFlowKind.Normal);


    private readonly IReadOnlyDictionary<IControlFlowNode, ControlFlowKind> items;
    public int Count => items.Count;
    public IEnumerable<IControlFlowNode> Keys => items.Keys;
    public IEnumerable<ControlFlowKind> Values => items.Values;

    private ControlFlowSet(IEnumerable<IControlFlowNode> nodes, ControlFlowKind kind)
    {
        items = nodes.ToDictionary(Functions.Identity, _ => kind);
    }

    private ControlFlowSet()
    {
        items = FixedDictionary<IControlFlowNode, ControlFlowKind>.Empty;
    }

    /// <remarks>CAUTION: this constructor takes ownership of the collection. No external references
    /// should be kept.</remarks>
    private ControlFlowSet(IReadOnlyDictionary<IControlFlowNode, ControlFlowKind> items)
    {
        this.items = items;
    }

    public ControlFlowKind this[IControlFlowNode key] => throw new System.NotImplementedException();

    public bool Contains(IControlFlowNode item) => items.ContainsKey(item);
    bool IReadOnlyDictionary<IControlFlowNode, ControlFlowKind>.ContainsKey(IControlFlowNode key) => items.ContainsKey(key);

    public bool TryGetValue(IControlFlowNode key, out ControlFlowKind value)
        => items.TryGetValue(key, out value);

    public IEnumerator<IControlFlowNode> GetEnumerator() => items.Keys.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => items.Keys.GetEnumerator();

    IEnumerator<KeyValuePair<IControlFlowNode, ControlFlowKind>>
        IEnumerable<KeyValuePair<IControlFlowNode, ControlFlowKind>>.GetEnumerator()
        => items.GetEnumerator();

    public ControlFlowSet Union(ControlFlowSet other)
    {
        Dictionary<IControlFlowNode, ControlFlowKind>? set = null;
        foreach (var (node, kind) in other.items)
        {
            if (items.TryGetValue(node, out var existing))
            {
                if (existing == kind)
                    continue;
                throw new InvalidOperationException(
                    $"Cannot union two {nameof(ControlFlowSet)}s with different kinds for node.");
            }
            set ??= new(items);
            set[node] = kind;
        }
        return set is null ? this : new(set);
    }
}