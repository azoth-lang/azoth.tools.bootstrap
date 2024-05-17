using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

/// <summary>
/// A scope defined by a declaration other than a namespace or using directive.
/// </summary>
internal class DeclarationScope : LexicalScope
{
    public override PackageNameScope PackageNames { get; }
    private readonly LexicalScope parent;
    private readonly FixedDictionary<StandardName, IFixedSet<INamedDeclarationNode>> declarations;

    internal DeclarationScope(LexicalScope parent, IEnumerable<INamedDeclarationNode> declarations)
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
