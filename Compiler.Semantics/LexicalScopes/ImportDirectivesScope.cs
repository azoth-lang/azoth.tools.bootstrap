using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

/// <summary>
/// A scope created by a collection of import directives.
/// </summary>
public sealed class ImportDirectivesScope : NamespaceSearchScope
{
    private readonly NamespaceScope parent;
    private readonly IFixedSet<NamespaceScope> importScopes;
    public override PackageNameScope PackageNames => parent.PackageNames;

    internal ImportDirectivesScope(NamespaceScope parent, IEnumerable<NamespaceScope> importScopes)
    {
        this.parent = parent;
        this.importScopes = importScopes.ToFixedSet();
    }

    public override NamespaceScope? GetChildNamespaceScope(IdentifierName namespaceName)
    {
        // We don't bother to cache these as it is very unlikely that two namespaces with the same
        // name will be created in the same lexical scope.

        var childScope = parent.GetChildNamespaceScope(namespaceName);
        if (childScope is null) return null;
        return new NamespaceScope(this, childScope);
    }

    public override IEnumerable<TDeclaration> Lookup<TDeclaration>(OrdinaryName name)
        => importScopes.SelectMany(s => s.LookupInNamespaceOnly<TDeclaration>(name))
                       .FallbackIfEmpty(() => parent.Lookup<TDeclaration>(name));
}
