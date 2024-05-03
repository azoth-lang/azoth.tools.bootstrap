using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public readonly record struct ConditionalSymbolScopes(SymbolScope True, SymbolScope False)
{
    public static ConditionalSymbolScopes Unconditional(SymbolScope scope) =>
        new ConditionalSymbolScopes(scope, scope);

    public ConditionalSymbolScopes Swapped() => new ConditionalSymbolScopes(False, True);
}
