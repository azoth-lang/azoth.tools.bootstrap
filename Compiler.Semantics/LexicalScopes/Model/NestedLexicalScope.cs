using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

public sealed class NestedLexicalScope : LexicalScope
{
    private readonly LexicalScope parent;
    public override PackageNameScope PackageNames => parent.PackageNames;
    private readonly IFixedSet<IDeclarationSymbolNode> containingDeclarations;

    public NestedLexicalScope(LexicalScope parent, IEnumerable<IDeclarationSymbolNode> containingDeclarations)
    {
        this.parent = parent;
        this.containingDeclarations = containingDeclarations.ToFixedSet();
    }

    public NestedLexicalScope(LexicalScope parent, params IDeclarationSymbolNode[] containingDeclarations)
      : this(parent, containingDeclarations.AsEnumerable())
    {
    }

    public override IEnumerable<ISymbolNode> Lookup(TypeName name, bool includeNested = true)
        => throw new System.NotImplementedException();
}
