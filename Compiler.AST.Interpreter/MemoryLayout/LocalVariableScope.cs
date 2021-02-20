using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout
{
    internal class LocalVariableScope
    {
        private readonly LocalVariableScope? enclosingScope;
        private readonly IDictionary<NamedBindingSymbol, AzothValue> values = new Dictionary<NamedBindingSymbol, AzothValue>();

        public LocalVariableScope(LocalVariableScope? enclosingScope = null)
        {
            this.enclosingScope = enclosingScope;
        }

        public AzothValue this[NamedBindingSymbol symbol]
        {
            get
            {
                if (values.TryGetValue(symbol, out var value)) return value;

                if (enclosingScope is null)
                    throw new InvalidOperationException($"Value for variable '{symbol}' not defined ");
                return enclosingScope[symbol];
            }
        }

        public void Add(NamedBindingSymbol symbol, AzothValue value)
        {
            values.Add(symbol, value);
        }
    }
}
