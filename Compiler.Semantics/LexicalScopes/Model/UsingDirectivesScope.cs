using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

/// <summary>
/// A scope created by a collection of using directives.
/// </summary>
internal class UsingDirectivesScope : LexicalScope
{
    private readonly NamespaceScope parent;
    private readonly IFixedSet<NamespaceScope> usingScopes;
    public override PackageNameScope PackageNames => parent.PackageNames;

    public UsingDirectivesScope(NamespaceScope parent, IEnumerable<NamespaceScope> usingScopes)
    {
        this.parent = parent;
        this.usingScopes = usingScopes.ToFixedSet();
    }

    public override NamespaceScope? CreateChildNamespaceScope(IdentifierName namespaceName)
        => throw new System.NotImplementedException();

    public override IEnumerable<ISymbolNode> Lookup(TypeName name)
        => usingScopes.SelectMany(s => s.Lookup(name, includeNested: false))
                      .FallbackIfEmpty(() => parent.Lookup(name));
}
