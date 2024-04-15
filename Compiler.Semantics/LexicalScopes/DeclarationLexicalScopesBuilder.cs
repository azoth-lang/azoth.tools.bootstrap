using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Framework;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public partial class DeclarationLexicalScopesBuilder
{
    private DeclarationLexicalScopesBuilder() { }

    private partial To.Package Transform(From.Package from)
    {
        var scope = new PackageReferenceScope(from.Symbol,
            from.References.ToFixedDictionary(r => r.AliasOrName, r => r.Package.SymbolTree));
        return Create(from, scope, scope);
    }

    private partial To.CompilationUnit Transform(From.CompilationUnit from, PackageReferenceScope containingScope)
    {
        // TODO add info about namespace and using directives
        var scope = new DeclarationScope(containingScope);
        return Create(from, scope);
    }

    private partial To.NamespaceDeclaration Transform(
        From.NamespaceDeclaration from,
        DeclarationLexicalScope containingScope)
    {
        // TODO add info about namespace and using directives
        var scope = new DeclarationScope(containingScope);
        return Create(from, containingScope, scope);
    }

    private partial To.TypeDeclaration Transform(
        From.TypeDeclaration from,
        DeclarationLexicalScope containingScope)
    {
        // TODO add info about scope
        var scope = new DeclarationScope(containingScope);
        return Create(from, containingScope, scope);
    }
}
