using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

public class PackagesScope : LexicalScope
{
    internal override PackagesScope ContainingPackagesScope => this;
    public PackageSymbol CurrentPackage { get; }
    private readonly FixedDictionary<IdentifierName, PackageSymbol> packageAliases;

    public PackagesScope(PackageSymbol currentPackage, FixedDictionary<IdentifierName, PackageSymbol> packageAliases)
    {
        CurrentPackage = currentPackage;
        this.packageAliases = packageAliases;
    }

    public override IEnumerable<IPromise<Symbol>> Lookup(TypeName name, bool includeNested = true)
        => Enumerable.Empty<IPromise<Symbol>>();
}
