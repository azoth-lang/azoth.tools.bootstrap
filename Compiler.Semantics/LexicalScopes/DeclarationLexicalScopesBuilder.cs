using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Framework;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal partial class DeclarationLexicalScopesBuilder
{
    private DeclarationLexicalScopesBuilder() { }

    private partial To.Package TransformPackage(From.Package from)
    {
        var scope = new PackageReferenceScope(from.Symbol,
            from.References.ToFixedDictionary(r => r.AliasOrName, r => r.Package.SymbolTree));
        return CreatePackage(from, lexicalScope: scope, childContainingScope: scope);
    }

    private partial To.CompilationUnit TransformCompilationUnit(
        From.CompilationUnit from, PackageReferenceScope containingScope)
    {
        // TODO add info about namespace and using directives
        var scope = new DeclarationScope(containingScope);
        return CreateCompilationUnit(from, lexicalScope: scope, childContainingScope: scope);
    }

    private partial To.NamespaceDeclaration TransformNamespaceDeclaration(
        From.NamespaceDeclaration from,
        DeclarationLexicalScope containingScope)
    {
        // TODO add info about namespace and using directives
        var scope = new DeclarationScope(containingScope);
        return CreateNamespaceDeclaration(from, containingScope: containingScope, newScope: scope, childContainingScope: scope);
    }

    private partial To.TypeDeclaration TransformTypeDeclaration(
        From.TypeDeclaration from,
        DeclarationLexicalScope containingScope)
    {
        // TODO add info about scope
        var scope = new DeclarationScope(containingScope);
        return CreateTypeDeclaration(from, containingScope: containingScope, newScope: scope, childContainingScope: scope);
    }
}
