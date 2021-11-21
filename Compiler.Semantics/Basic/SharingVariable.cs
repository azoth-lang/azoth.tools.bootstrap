using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic
{
    /// <summary>
    /// A variable that can participate in sharing.
    /// </summary>
    public readonly struct SharingVariable
    {
        public static readonly SharingVariable Result = default;

        private readonly BindingSymbol symbol;

        public SharingVariable(BindingSymbol symbol)
        {
            this.symbol = symbol;
        }
    }
}
