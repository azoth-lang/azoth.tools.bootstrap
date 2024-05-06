using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

public sealed class NamespaceScope : LexicalScope
{
    private readonly NamespaceScope? parent;
    public override PackageNameScope PackageNames { get; }
    private readonly IFixedSet<INamespaceSymbolNode> namespaceDeclarations;
    private readonly Dictionary<IdentifierName, NamespaceScope> childScopes = new();

    public NamespaceScope(PackageNameScope parent, IEnumerable<INamespaceSymbolNode> namespaceDeclarations)
    {
        PackageNames = parent;
        this.namespaceDeclarations = namespaceDeclarations.ToFixedSet();
    }

    public NamespaceScope(NamespaceScope parent, IEnumerable<INamespaceSymbolNode> namespaceDeclarations)
    {
        this.parent = parent;
        PackageNames = parent.PackageNames;
        this.namespaceDeclarations = namespaceDeclarations.ToFixedSet();
    }

    public override NamespaceScope? CreateChildNamespaceScope(IdentifierName namespaceName)
    {
        if (childScopes.TryGetValue(namespaceName, out var childScope)) return childScope;

        var childDeclarations = namespaceDeclarations.SelectMany(d => d.MembersNamed(namespaceName))
                                                     .OfType<INamespaceSymbolNode>().ToFixedSet();
        if (childDeclarations.Count == 0) return null;
        childScope = new NamespaceScope(this, childDeclarations);
        childScopes.Add(namespaceName, childScope);
        return childScope;
    }

    public override IEnumerable<ISymbolNode> Lookup(TypeName name) => Lookup(name, true);

    public IEnumerable<ISymbolNode> Lookup(TypeName name, bool includeNested)
        => throw new System.NotImplementedException();
}
