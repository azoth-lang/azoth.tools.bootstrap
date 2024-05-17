using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

internal class BasicScope : LexicalScope
{
    public override PackageNameScope PackageNames { get; }
    private readonly LexicalScope parent;
    private readonly FixedDictionary<StandardName, IFixedSet<INamedDeclarationNode>> declarations;

    internal BasicScope(LexicalScope parent, IEnumerable<INamedDeclarationNode> declarations)
    {
        this.parent = parent;
        PackageNames = parent.PackageNames;
        this.declarations = declarations.GroupBy(n => n.Name)
                                        .ToFixedDictionary(g => g.Key, g => g.ToFixedSet());
    }

    public override IEnumerable<IDeclarationNode> Lookup(StandardName name)
    {
        if (declarations.TryGetValue(name, out var nodes))
            return nodes;
        return parent.Lookup(name);
    }
}
