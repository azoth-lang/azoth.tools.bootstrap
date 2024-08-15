using Azoth.Tools.Bootstrap.Compiler.API.Tree;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.API;

public sealed class PackageReference(IdentifierName nameOrAlias, IPackageSymbols packageSymbols, bool isTrusted)
{
    public IdentifierName NameOrAlias { get; } = nameOrAlias;
    public IPackageSymbols PackageSymbols { get; } = packageSymbols;
    public bool IsTrusted { get; } = isTrusted;

    internal IPackageReferenceSyntax ToSyntax()
        => new PackageReferenceSyntax(NameOrAlias, PackageSymbols, IsTrusted);
}
