using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.API.Tree;

internal sealed class PackageReferenceSyntax : IPackageReferenceSyntax<Package>
{
    public IdentifierName AliasOrName { get; }
    public Package Package { get; }
    public bool IsTrusted { get; }

    public PackageReferenceSyntax(IdentifierName aliasOrName, Package package, bool isTrusted)
    {
        AliasOrName = aliasOrName;
        Package = package;
        IsTrusted = isTrusted;
    }
}
