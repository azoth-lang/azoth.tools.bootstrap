using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Async;
using InlineMethod;
using Microsoft.Extensions.ObjectPool;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

[StructLayout(LayoutKind.Auto)]
internal readonly struct LocalVariables
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct Scope : IDisposable
    {
        private readonly DefaultObjectPool<VariableStack> pool;
        private readonly LocalVariables localVariables;

        private Scope(
            DefaultObjectPool<VariableStack> pool,
            ConcurrentDictionary<IBindingNode, int> bindingIndexes)
        {
            localVariables = new(bindingIndexes, pool.Get());
            this.pool = pool;
        }

        [Inline(InlineBehavior.Remove)]
        public void Add(IBindingNode binding, AzothValue value)
            => localVariables.Add(binding, value);

        [Inline(InlineBehavior.Remove)]
        public static implicit operator LocalVariables(Scope scope) => scope.localVariables;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => pool.Return(localVariables.variableStack);

        internal sealed class Pool
        {
            private readonly ConcurrentDictionary<IBindingNode, int> bindingIndexes
                = new(ReferenceEqualityComparer.Instance);
            private readonly DefaultObjectPool<VariableStack> pool
                = new(new Policy(), Environment.ProcessorCount * 10);

            public Scope CreateRoot() => new(pool, bindingIndexes);

            private class Policy : IPooledObjectPolicy<VariableStack>
            {
                private const int MaxCapacityToReturn = 1024 + 1;
                public VariableStack Create() => new();

                public bool Return(VariableStack stack)
                {
                    if (stack.Capacity > MaxCapacityToReturn) return false;
                    stack.Clear();
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

        [Inline(InlineBehavior.Remove)]
        public void Add(IBindingNode binding, AzothValue value)
            => localVariables.Add(binding, value);

        [Inline(InlineBehavior.Remove)]
        public static implicit operator LocalVariables(NestedScope scope) => scope.localVariables;

        public void Dispose()
        {
            var stack = localVariables.variableStack;
            // Remove any values added in this scope
            stack.RemoveRange(beforeScopeCount, stack.Count - beforeScopeCount);
        }
    }

    private readonly ConcurrentDictionary<IBindingNode, int> bindingIndexes;
    private readonly VariableStack variableStack;
    public AsyncScope? AsyncScope { [DebuggerStepThrough] get; }

    private LocalVariables(
        ConcurrentDictionary<IBindingNode, int> bindingIndexes,
        VariableStack variableStack,
        AsyncScope? asyncScope = null)
    {
        this.bindingIndexes = bindingIndexes;
        this.variableStack = variableStack;
        AsyncScope = asyncScope;
    }

    [Inline(InlineBehavior.Remove)]
    public NestedScope CreateNestedScope(AsyncScope? asyncScope = null)
        => new NestedScope(this, asyncScope);

    public AzothValue this[IBindingNode binding]
    {
        [Inline(InlineBehavior.Remove)]
        get => variableStack[bindingIndexes[binding]];
        [Inline(InlineBehavior.Remove)]
        set => variableStack[bindingIndexes[binding]] = value;
    }

    [Inline(InlineBehavior.Remove)]
    public void Add(IBindingNode binding, AzothValue value)
    {
        var index = variableStack.Count;
        var existingIndex = bindingIndexes.GetOrAdd(binding, index);
        if (existingIndex != index)
            throw new InvalidOperationException("Cannot change index of a binding");
        variableStack.Add(value);
    }

    [Inline(InlineBehavior.Remove)]
    public AzothRef Ref(IBindingNode binding)
        => new(variableStack, bindingIndexes[binding]);
}
