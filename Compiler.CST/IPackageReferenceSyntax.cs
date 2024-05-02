using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public interface IPackageReferenceSyntax
{
    IdentifierName AliasOrName { get; }
    IPackageSymbols Package { get; }
    bool IsTrusted { get; }
}


public interface IPackageReferenceSyntax<out TPackage> : IPackageReferenceSyntax
    where TPackage : IPackageSymbols
{
    new TPackage Package { get; }
    IPackageSymbols IPackageReferenceSyntax.Package => Package;
}
