using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public readonly record struct ConditionalLexicalScopes(LexicalScope True, LexicalScope False)
{
    public static ConditionalLexicalScopes Unconditional(LexicalScope scope) =>
        new ConditionalLexicalScopes(scope, scope);

    public ConditionalLexicalScopes Swapped() => new ConditionalLexicalScopes(False, True);
}
