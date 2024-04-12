using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

public sealed class DeclarationScope : DeclarationLexicalScope
{
    public override PackageReferenceScope RootScope { get; }
    private readonly DeclarationLexicalScope containingScope;

    public DeclarationScope(DeclarationLexicalScope containingScope)
    {
        this.containingScope = containingScope;
        RootScope = containingScope.RootScope;
    }

    public override IEnumerable<Symbol> ResolveInReferences(StandardName name)
        => throw new System.NotImplementedException();
}
