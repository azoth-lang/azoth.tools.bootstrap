using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public sealed class ContributorCollection<T>
    where T : notnull
{
    public static Scope Create(out ContributorCollection<T> contributors)
    {
        contributors = new();
        return new Scope(contributors);
    }

    private ContributorCollection() { }

    private readonly HashSet<T> contributorsToAll = new();
    private readonly ConcurrentDictionary<T, HashSet<T>> contributors = new();
    private bool complete;

    public void AddToAll(T contributor)
    {
        if (complete)
            throw new InvalidOperationException("Contributor collection is already complete.");
        contributorsToAll.Add(contributor);
    }

    public void Add(T target, T contributor)
    {
        if (complete)
            throw new InvalidOperationException("Contributor collection is already complete.");
        var contributorsToTarget = contributors.GetOrAdd(target, NewSet);
        contributorsToTarget.Add(contributor);
    }

    private static HashSet<T> NewSet(T _) => new();

    public void MarkComplete() => complete = true;

    public IEnumerable<T> Remove(T target)
    {
        // Double-checked locking to check if the collection is complete, or wait for it to be complete
        if (!complete)
            using (new Scope(this))
            {
                if (!complete)
                    throw new InvalidOperationException("Contributor collection was not successfully completed.");
            }

        if (contributors.TryRemove(target, out var contributorsToTarget))
            return contributorsToAll.Concat(contributorsToTarget);
        return contributorsToAll;
    }

    public readonly ref struct Scope
    {
        private readonly object monitor;

        public Scope(ContributorCollection<T> contributors)
        {
            monitor = contributors.contributorsToAll;
            Monitor.Enter(monitor);
        }

        public void Dispose() => Monitor.Exit(monitor);
    }
}
