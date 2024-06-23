using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public readonly struct RewritableAttributeFunction<TNode, T> : ICyclicAttributeFunction<TNode, T>
    where T : IChildTreeNode?
{
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Compute(TNode _, T current) => (T)current!.Rewrite()!;
}
