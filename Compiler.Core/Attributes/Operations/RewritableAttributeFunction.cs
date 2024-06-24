using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public readonly struct RewritableAttributeFunction<TNode, T> : ICyclicAttributeFunction<TNode, T>
    where TNode : ITreeNode
    where T : IChildTreeNode<TNode>?
{
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Compute(TNode node, T current)
    {
        var child = (T?)current!.Rewrite();
        child?.SetParent(node);
        return child!;
    }
}
