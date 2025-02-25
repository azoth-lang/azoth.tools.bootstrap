using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Async;
using Microsoft.Extensions.ObjectPool;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

[StructLayout(LayoutKind.Auto)]
internal readonly struct LocalVariables
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct Scope : IDisposable
    {
        private readonly DefaultObjectPool<List<AzothValue>> pool;
        private readonly LocalVariables localVariables;

        private Scope(
            DefaultObjectPool<List<AzothValue>> pool,
            ConcurrentDictionary<IBindingNode, int> bindingIndexes)
        {
            localVariables = new(bindingIndexes, pool.Get());
            this.pool = pool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(IBindingNode binding, AzothValue value)
            => localVariables.Add(binding, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LocalVariables(Scope scope) => scope.localVariables;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => pool.Return(localVariables.variableStack);

        internal sealed class Pool
        {
            private readonly ConcurrentDictionary<IBindingNode, int> bindingIndexes
                = new(ReferenceEqualityComparer.Instance);
            private readonly DefaultObjectPool<List<AzothValue>> pool
                = new(new Policy(), Environment.ProcessorCount * 10);

            public Scope CreateRoot() => new(pool, bindingIndexes);

            private class Policy : IPooledObjectPolicy<List<AzothValue>>
            {
                private const int MaxCapacityToReturn = 1024 + 1;
                public List<AzothValue> Create() => new List<AzothValue>();

                public bool Return(List<AzothValue> list)
                {
                    if (list.Capacity > MaxCapacityToReturn) return false;
                    list.Clear();
                    return true;
                }
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal readonly struct NestedScope : IDisposable
    {
        private readonly LocalVariables localVariables;
        private readonly int beforeScopeCount;

        internal NestedScope(LocalVariables localVariables, AsyncScope? asyncScope = null)
        {
            this.localVariables = new(localVariables.bindingIndexes, localVariables.variableStack,
                localVariables.AsyncScope ?? asyncScope);
            beforeScopeCount = localVariables.variableStack.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(IBindingNode binding, AzothValue value)
            => localVariables.Add(binding, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LocalVariables(NestedScope scope) => scope.localVariables;

        public void Dispose()
        {
            var stack = localVariables.variableStack;
            // Remove any values added in this scope
            stack.RemoveRange(beforeScopeCount, stack.Count - beforeScopeCount);
        }
    }

    private readonly ConcurrentDictionary<IBindingNode, int> bindingIndexes;
    private readonly List<AzothValue> variableStack;
    public AsyncScope? AsyncScope { [DebuggerStepThrough] get; }

    private LocalVariables(
        ConcurrentDictionary<IBindingNode, int> bindingIndexes,
        List<AzothValue> variableStack,
        AsyncScope? asyncScope = null)
    {
        this.bindingIndexes = bindingIndexes;
        this.variableStack = variableStack;
        AsyncScope = asyncScope;
    }

    public NestedScope CreateNestedScope(AsyncScope? asyncScope = null)
        => new NestedScope(this, asyncScope);

    public AzothValue this[IBindingNode binding]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => variableStack[bindingIndexes[binding]];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => variableStack[bindingIndexes[binding]] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(IBindingNode binding, AzothValue value)
    {
        var index = variableStack.Count;
        var existingIndex = bindingIndexes.GetOrAdd(binding, index);
        if (existingIndex != index)
            throw new InvalidOperationException("Cannot change index of a binding");
        variableStack.Add(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AzothRef Ref(IBindingNode binding)
        => new(variableStack, bindingIndexes[binding]);
}
