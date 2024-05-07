using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

/// <summary>
/// A scope created by a collection of using directives.
/// </summary>
public class UsingDirectivesScope : LexicalScope
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
    {
        // We don't bother to cache these as it is very unlikely that two namespaces with the same
        // name will be created in the same lexical scope.

        var childScope = parent.CreateChildNamespaceScope(namespaceName);
        if (childScope is null) return null;
        return new NamespaceScope(this, childScope);
    }

    public override IEnumerable<ISymbolNode> Lookup(StandardName name)
        => usingScopes.SelectMany(s => s.LookupInNamespaceOnly(name))
                      .FallbackIfEmpty(() => parent.Lookup(name));
}
