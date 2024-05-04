using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

/// <summary>
/// A lexical scope that supports lookup of symbol nodes by name.
/// </summary>
public sealed class LexicalScope
{
    private readonly LexicalScope? parent;
    private readonly IFixedSet<IDeclarationSymbolNode> containingDeclarations;
    public PackageNameScope PackageNames { get; }
    private readonly Dictionary<IdentifierName, LexicalScope> childScopes = new();

    public LexicalScope(PackageNameScope parent, IEnumerable<IDeclarationSymbolNode> containingDeclarations)
    {
        PackageNames = parent;
        this.containingDeclarations = containingDeclarations.ToFixedSet();
    }

    public LexicalScope(PackageNameScope parent, params IDeclarationSymbolNode[] containingDeclarations)
        : this(parent, containingDeclarations.AsEnumerable()) { }

    public LexicalScope(LexicalScope parent, IEnumerable<IDeclarationSymbolNode> containingDeclarations)
    {
        this.parent = parent;
        PackageNames = parent.PackageNames;
        this.containingDeclarations = containingDeclarations.ToFixedSet();
    }

    public LexicalScope(LexicalScope parent, params IDeclarationSymbolNode[] containingDeclarations)
        : this(parent, containingDeclarations.AsEnumerable()) { }

    public IEnumerable<ISymbolNode> Lookup(TypeName name, bool includeNested = true)
        => throw new System.NotImplementedException();

    public LexicalScope? GetChildScope(IdentifierName namespaceName)
    {
        if (childScopes.TryGetValue(namespaceName, out var childScope))
            return childScope;

        var childDeclarations = containingDeclarations.SelectMany(d => d.MembersNamed(namespaceName))
                                                      .OfType<IDeclarationWithMembersSymbolNode>()
                                                      .ToFixedSet();
        if (childDeclarations.Count == 0)
            return null;
        childScope = new LexicalScope(this, Enumerable.Empty<IDeclarationSymbolNode>());
        childScopes.Add(namespaceName, childScope);
        return childScope;
    }
}
