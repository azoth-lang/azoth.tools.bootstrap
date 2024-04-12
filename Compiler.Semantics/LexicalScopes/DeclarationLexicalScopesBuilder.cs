using System;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public partial class DeclarationLexicalScopesBuilder
{
    private DeclarationLexicalScopesBuilder() { }

    private partial To.Package Transform(From.Package from)
        => throw new NotImplementedException();
}
