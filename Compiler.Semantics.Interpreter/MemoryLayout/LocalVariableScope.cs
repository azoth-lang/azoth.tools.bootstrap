using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Async;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class LocalVariableScope
{
    private readonly LocalVariableScope? enclosingScope;
    private readonly Dictionary<IBindingNode, AzothValue> values = new();
    private readonly AsyncScope? asyncScope;

    public AsyncScope? AsyncScope => asyncScope ?? enclosingScope?.AsyncScope;

    public LocalVariableScope(LocalVariableScope? enclosingScope = null, AsyncScope? asyncScope = null)
    {
        this.enclosingScope = enclosingScope;
        this.asyncScope = asyncScope;
    }

    public AzothValue this[IBindingNode binding]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (values.TryGetValue(binding, out var value)) return value;
            return enclosingScope?[binding]
                ?? throw new InvalidOperationException($"Value for variable '{binding.Syntax}' not defined ");
        }
        set
        {
            if (values.TryUpdate(binding, value))
                return;
            if (enclosingScope is null)
                throw new InvalidOperationException($"Value for variable '{binding.Syntax}' not defined ");
            enclosingScope[binding] = value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(IBindingNode binding, AzothValue value) => values.Add(binding, value);
}
