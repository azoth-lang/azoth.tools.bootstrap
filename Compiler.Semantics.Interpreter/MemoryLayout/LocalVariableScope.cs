using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Async;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal class LocalVariableScope
{
    private readonly LocalVariableScope? enclosingScope;
    private readonly IDictionary<IBindingNode, AzothValue> values = new Dictionary<IBindingNode, AzothValue>();
    private readonly AsyncScope? asyncScope;

    public AsyncScope? AsyncScope => asyncScope ?? enclosingScope?.AsyncScope;

    public LocalVariableScope(LocalVariableScope? enclosingScope = null, AsyncScope? asyncScope = null)
    {
        this.enclosingScope = enclosingScope;
        this.asyncScope = asyncScope;
    }

    public AzothValue this[IBindingNode binding]
    {
        get
        {
            if (values.TryGetValue(binding, out var value)) return value;
            if (enclosingScope is null)
                throw new InvalidOperationException($"Value for variable '{binding.Syntax}' not defined ");
            return enclosingScope[binding];
        }
        set
        {
            if (values.ContainsKey(binding))
            {
                values[binding] = value;
                return;
            }
            if (enclosingScope is null)
                throw new InvalidOperationException($"Value for variable '{binding.Syntax}' not defined ");
            enclosingScope[binding] = value;
        }
    }

    public void Add(IBindingNode binding, AzothValue value) => values.Add(binding, value);
}
