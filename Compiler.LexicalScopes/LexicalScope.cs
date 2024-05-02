using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

/// <summary>
/// Lookup things by name in lexical scopes
/// </summary>
public abstract class LexicalScope
{
    internal abstract PackagesScope ContainingPackagesScope { get; }

    public abstract IEnumerable<IPromise<Symbol>> Lookup(TypeName name, bool includeNested = true);
}
