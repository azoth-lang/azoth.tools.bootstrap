using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

internal class BasicScope : LexicalScope
{
    public override PackageNameScope PackageNames { get; }
    private readonly LexicalScope parent;
    private readonly FixedDictionary<StandardName, IFixedSet<INamedDeclarationNode>> nodesInScope;

    public BasicScope(LexicalScope parent, IEnumerable<INamedDeclarationNode> nodesInScope)
    {
        this.parent = parent;
        PackageNames = parent.PackageNames;
        this.nodesInScope = nodesInScope.GroupBy(n => n.Name)
                                        .ToFixedDictionary(g => g.Key, g => g.ToFixedSet());
    }

    // TODO refactor so this method doesn't need to be implemented?
    public override NamespaceScope? CreateChildNamespaceScope(IdentifierName namespaceName)
        => throw new NotSupportedException();

    public override IEnumerable<IDeclarationNode> Lookup(StandardName name)
    {
        if (nodesInScope.TryGetValue(name, out var nodes))
            return nodes;
        return parent.Lookup(name);
    }
}
