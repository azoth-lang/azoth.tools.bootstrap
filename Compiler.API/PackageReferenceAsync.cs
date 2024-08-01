using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.API;

public sealed class PackageReferenceAsync(IdentifierName nameOrAlias, Task<Package> package, bool isTrusted)
{
    public IdentifierName NameOrAlias { get; } = nameOrAlias;
    public Task<Package> Package { get; } = package;
    public bool IsTrusted { get; } = isTrusted;

    internal async Task<IPackageReferenceSyntax> ToSyntaxAsync()
        => new PackageReference(NameOrAlias, await Package.ConfigureAwait(false), IsTrusted).ToSyntax();
}
