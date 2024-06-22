using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public interface IAttributeOperations<TNode, T> : IEqualityComparer<T>
{
    T Compute(TNode node, IInheritanceContext ctx);

    bool CompareExchange(ref T location, T value, T? comparand, out T previous);
}
