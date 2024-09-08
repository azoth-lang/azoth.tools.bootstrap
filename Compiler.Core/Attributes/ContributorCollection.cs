using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public sealed class ContributorCollection<T>
    where T : notnull
{
    private readonly HashSet<T> contributorsToAll = new();
    private readonly ConcurrentDictionary<T, HashSet<T>> contributors = new();
    private CollectionState state = CollectionState.Unpopulated;

    public void EnsurePopulated(Action<ContributorCollection<T>> populate)
    {
        if (state == CollectionState.Populated)
            return;
        lock (contributorsToAll)
        {
            switch (state)
            {
                default:
                    throw ExhaustiveMatch.Failed(state);
                case CollectionState.Unpopulated:
                    state = CollectionState.InProgress;
                    break;
                case CollectionState.InProgress:
                    throw new InvalidOperationException("Contributor collection was not successfully completed.");
                case CollectionState.Populated:
                    return;
            }
            populate(this);
            state = CollectionState.Populated;
        }
    }

    public void AddToAll(T contributor)
    {
        if (state != CollectionState.InProgress)
            throw new InvalidOperationException("Contributor collection must be in progress to add to it.");
        contributorsToAll.Add(contributor);
    }

    public void AddToRange(IEnumerable<T> targets, T contributor)
    {
        if (state != CollectionState.InProgress)
            throw new InvalidOperationException("Contributor collection must be in progress to add to it.");
        foreach (var target in targets)
            contributors.GetOrAdd(target, NewSet).Add(contributor);
    }

    public void Add(T target, T contributor)
    {
        if (state != CollectionState.InProgress)
            throw new InvalidOperationException("Contributor collection must be in progress to add to it.");
        var contributorsToTarget = contributors.GetOrAdd(target, NewSet);
        contributorsToTarget.Add(contributor);
    }

    private static HashSet<T> NewSet(T _) => new();

    public IEnumerable<T> Remove(T target)
    {
        if (state != CollectionState.Populated)
            throw new InvalidOperationException("Contributor collection was not successfully populated.");

        if (contributors.TryRemove(target, out var contributorsToTarget))
            return contributorsToAll.Concat(contributorsToTarget);
        return contributorsToAll;
    }

    private enum CollectionState
    {
        Unpopulated = 0,
        InProgress,
        Populated
    }
}
