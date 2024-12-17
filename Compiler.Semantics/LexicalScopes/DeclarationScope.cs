using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

/// <summary>
/// A scope defined by a declaration other than a namespace or using directive.
/// </summary>
internal class DeclarationScope : LexicalScope
{
    public override PackageNameScope PackageNames { get; }
    private readonly LexicalScope parent;
    private readonly FixedDictionary<TypeName, IFixedSet<INamedDeclarationNode>> declarations;

    internal DeclarationScope(LexicalScope parent, IEnumerable<INamedDeclarationNode> declarations)
    {
        this.parent = parent;
        PackageNames = parent.PackageNames;
        this.declarations = declarations.GroupBy(n => n.Name)
                                        .ToFixedDictionary(g => g.Key, g => g.ToFixedSet());
    }

    internal DeclarationScope(LexicalScope parent, INamedDeclarationNode declaration, params INamedDeclarationNode[] declarations)
        : this(parent, declarations.Prepend(declaration))
    {
    }

    public override IEnumerable<IDeclarationNode> Lookup(OrdinaryName name)
    {
        if (declarations.TryGetValue(name, out var nodes))
            return nodes;
        return parent.Lookup(name);
    }

    /// <remarks>Declarations can contain the <see cref="BuiltInTypeName.Self"/> declaration.</remarks>
    public override ITypeDeclarationNode? Lookup(BuiltInTypeName name)
    {
        if (declarations.TryGetValue(name, out var nodes))
            return nodes.Cast<ITypeDeclarationNode>().Single();
        // The parent could be a declaration scope and needs checked.
        return parent.Lookup(name);
    }
}
