using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

public sealed class NamespaceName : IEquatable<NamespaceName>
{
    public static readonly NamespaceName Global = new(FixedList<SimpleName>.Empty);

    public FixedList<SimpleName> Segments { [DebuggerStepThrough] get; }

    public NamespaceName(FixedList<SimpleName> segments)
    {
        Segments = segments;
    }

    public NamespaceName(IEnumerable<SimpleName> segments)
        : this(segments.ToFixedList()) { }

    public NamespaceName(params string[] segments)
        : this(segments.Select(s => new SimpleName(s)).ToFixedList()) { }

    public NamespaceName(SimpleName segment)
    {
        Segments = segment.Yield().ToFixedList();
    }

    public NamespaceName Qualify(NamespaceName name)
        => new(Segments.Concat(name.Segments));

    public IEnumerable<NamespaceName> NamespaceNames()
    {
        yield return Global;
        for (int n = 1; n <= Segments.Count; n++)
            yield return new NamespaceName(Segments.Take(n));
    }

    public override string ToString() => string.Join('.', Segments);

    #region Equals
    public bool Equals(NamespaceName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Segments.ItemsEquals<TypeName>(other.Segments);
    }

    public override bool Equals(object? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is NamespaceName name && Equals(name);
    }

    public override int GetHashCode() => HashCode.Combine(Segments);

    public static bool operator ==(NamespaceName? left, NamespaceName? right)
        => Equals(left, right);

    public static bool operator !=(NamespaceName? left, NamespaceName? right)
        => !Equals(left, right);
    #endregion

    public static implicit operator NamespaceName(SimpleName name) => new(name);

    public static implicit operator NamespaceName(string text) => new(text);

    public bool IsNestedIn(NamespaceName ns)
    {
        return ns.Segments.Count < Segments.Count
               && ns.Segments.SequenceEqual(Segments.Take(ns.Segments.Count));
    }
}
