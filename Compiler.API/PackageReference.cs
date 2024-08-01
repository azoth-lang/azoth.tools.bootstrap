using Azoth.Tools.Bootstrap.Compiler.API.Tree;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.API;

public sealed class PackageReference(IdentifierName nameOrAlias, IPackageSymbols package, bool isTrusted)
{
    public IdentifierName NameOrAlias { get; } = nameOrAlias;
    public IPackageSymbols Package { get; } = package;
    public bool IsTrusted { get; } = isTrusted;

    internal IPackageReferenceSyntax ToSyntax()
        => new PackageReferenceSyntax(NameOrAlias, Package, IsTrusted);
}
