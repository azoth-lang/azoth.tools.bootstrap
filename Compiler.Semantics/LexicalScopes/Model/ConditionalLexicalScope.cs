namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

public readonly record struct ConditionalLexicalScope(LexicalScope True, LexicalScope False)
{
    public static ConditionalLexicalScope Unconditional(LexicalScope scope)
        => new ConditionalLexicalScope(scope, scope);

    public ConditionalLexicalScope Swapped() => new ConditionalLexicalScope(False, True);
}
