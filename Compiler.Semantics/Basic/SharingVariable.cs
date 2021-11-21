using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic
{
    /// <summary>
    /// A variable that can participate in sharing. This is all the <see cref="BindingSymbol"/>
    /// plus a special value <see cref="Result"/> representing the result of an expression.
    /// </summary>
    public readonly struct SharingVariable : IEquatable<SharingVariable>
    {
        public static readonly SharingVariable Result = default;

        private readonly BindingSymbol? symbol;

        public SharingVariable(BindingSymbol symbol)
        {
            this.symbol = symbol;
        }

        public override int GetHashCode()
            => HashCode.Combine(symbol);

        public bool Equals(SharingVariable other)
        {
            if (symbol is null) return other.symbol is null;
            return symbol.Equals(other.symbol);
        }

        public override bool Equals(object? obj)
            => obj is SharingVariable other && Equals(other);

        public static bool operator ==(SharingVariable left, SharingVariable right)
            => left.Equals(right);

        public static bool operator !=(SharingVariable left, SharingVariable right)
            => !left.Equals(right);

        public static implicit operator SharingVariable(BindingSymbol symbol)
            => new(symbol);
    }
}
