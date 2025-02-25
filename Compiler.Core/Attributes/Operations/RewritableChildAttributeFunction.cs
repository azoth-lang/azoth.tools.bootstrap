using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

[StructLayout(LayoutKind.Auto)]
public readonly struct RewritableChildAttributeFunction<TNode, T> : ICyclicAttributeFunction<TNode, T>
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
