using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.API;

public sealed class PackageReferenceAsync(IdentifierName nameOrAlias, Task<IPackageSymbols> packageSymbols, bool isTrusted)
{
    public IdentifierName NameOrAlias { get; } = nameOrAlias;
    public Task<IPackageSymbols> PackageSymbols { get; } = packageSymbols;
    public bool IsTrusted { get; } = isTrusted;

    internal async Task<IPackageReferenceSyntax> ToSyntaxAsync()
        => new PackageReference(NameOrAlias, await PackageSymbols.ConfigureAwait(false), IsTrusted).ToSyntax();
}
