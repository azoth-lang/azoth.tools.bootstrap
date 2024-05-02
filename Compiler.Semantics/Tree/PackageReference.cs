using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PackageReference : Node
{
    public IPackageReferenceSyntax Syntax { get; }
    ISyntax Node.Syntax => Syntax;

    public PackageReference(IPackageReferenceSyntax syntax)
    {
        Syntax = syntax;
    }
}
