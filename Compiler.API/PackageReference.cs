using Azoth.Tools.Bootstrap.Compiler.API.Tree;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.API;

public sealed class PackageReference(IdentifierName nameOrAlias, Package package, bool isTrusted)
{
    public IdentifierName NameOrAlias { get; } = nameOrAlias;
    public Package Package { get; } = package;
    public bool IsTrusted { get; } = isTrusted;

    internal IPackageReferenceSyntax<Package> ToSyntax()
        => new PackageReferenceSyntax(NameOrAlias, Package, IsTrusted);
}
