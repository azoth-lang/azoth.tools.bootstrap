using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.API.Tree;

internal sealed class PackageReferenceSyntax : IPackageReferenceSyntax
{
    public IdentifierName AliasOrName { get; }
    public IPackageSymbols Package { get; }
    public bool IsTrusted { get; }

    public PackageReferenceSyntax(IdentifierName aliasOrName, IPackageSymbols package, bool isTrusted)
    {
        AliasOrName = aliasOrName;
        Package = package;
        IsTrusted = isTrusted;
    }
}
