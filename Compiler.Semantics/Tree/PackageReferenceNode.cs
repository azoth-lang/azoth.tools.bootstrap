using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PackageReferenceNode : SemanticNode, IPackageReferenceNode
{
    public override IPackageReferenceSyntax Syntax { get; }

    public IdentifierName AliasOrName => Syntax.AliasOrName;
    public IPackageSymbols Package => Syntax.Package;
    public bool IsTrusted => Syntax.IsTrusted;

    public PackageReferenceNode(IPackageReferenceSyntax syntax)
    {
        Syntax = syntax;
    }
}
