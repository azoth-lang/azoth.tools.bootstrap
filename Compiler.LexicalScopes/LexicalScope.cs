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

    public virtual PackageSymbol? LookupPackage(SimpleName name)
        => ContainingPackagesScope.LookupPackage(name);

    public abstract IEnumerable<IPromise<Symbol>> LookupInGlobalScope(Name name);

    public abstract IEnumerable<IPromise<Symbol>> Lookup(Name name, bool includeNested = true);
}
