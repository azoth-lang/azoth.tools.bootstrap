using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

public sealed class PackageNameScope : LexicalScope
{
    public override PackageNameScope PackageNames => this;

    public override IEnumerable<ISymbolNode> Lookup(TypeName name, bool includeNested = true)
        => Enumerable.Empty<ISymbolNode>();
}
