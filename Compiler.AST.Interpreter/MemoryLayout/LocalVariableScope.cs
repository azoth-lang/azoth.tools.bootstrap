using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.Async;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;

internal class LocalVariableScope
{
    private readonly LocalVariableScope? enclosingScope;
    private readonly IDictionary<BindingSymbol, AzothValue> values = new Dictionary<BindingSymbol, AzothValue>();
    private readonly AsyncScope? asyncScope;

    public AsyncScope? AsyncScope => asyncScope ?? enclosingScope?.AsyncScope;

    public LocalVariableScope(LocalVariableScope? enclosingScope = null, AsyncScope? asyncScope = null)
    {
        this.enclosingScope = enclosingScope;
        this.asyncScope = asyncScope;
    }

    public AzothValue this[BindingSymbol symbol]
    {
        get
        {
            if (values.TryGetValue(symbol, out var value)) return value;
            if (enclosingScope is null)
                throw new InvalidOperationException($"Value for variable '{symbol}' not defined ");
            return enclosingScope[symbol];
        }
        set
        {
            if (values.ContainsKey(symbol))
            {
                values[symbol] = value;
                return;
            }
            if (enclosingScope is null)
                throw new InvalidOperationException($"Value for variable '{symbol}' not defined ");
            enclosingScope[symbol] = value;
        }
    }

    public void Add(BindingSymbol symbol, AzothValue value) => values.Add(symbol, value);
}
