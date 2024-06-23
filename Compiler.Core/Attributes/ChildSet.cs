using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public static class ChildSet
{
    /// <summary>
    /// Attach a set of children that does not support rewriting.
    /// </summary>
    public static IFixedSet<TChild> Attach<TParent, TChild>(TParent parent, IEnumerable<TChild> children)
        where TParent : IParent
        where TChild : class, IChild<TParent>
    {
        var childSet = children.ToFixedSet();
        foreach (var child in childSet)
            Child.Attach(parent, child);
        return childSet;
    }
}
