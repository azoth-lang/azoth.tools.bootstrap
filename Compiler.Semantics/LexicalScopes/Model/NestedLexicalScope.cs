using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

public sealed class NestedLexicalScope : LexicalScope
{
    private readonly LexicalScope parent;
    public override PackageNameScope PackageNames => parent.PackageNames;

    public NestedLexicalScope(LexicalScope parent)
    {
        this.parent = parent;
    }

    public override IEnumerable<ISymbolNode> Lookup(TypeName name, bool includeNested = true)
        => throw new System.NotImplementedException();
}
