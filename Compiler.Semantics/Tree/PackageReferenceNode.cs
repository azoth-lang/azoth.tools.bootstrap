using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PackageReferenceNode : ChildNode, IPackageReferenceNode
{
    public override IPackageReferenceSyntax Syntax { get; }

    public IdentifierName AliasOrName => Syntax.AliasOrName;
    public IPackageSymbols PackageSymbols => Syntax.Package;
    public bool IsTrusted => Syntax.IsTrusted;

    private ValueAttribute<IPackageSymbolNode> symbolNode;
    public IPackageSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAspect.PackageReference_SymbolNode);

    public PackageReferenceNode(IPackageReferenceSyntax syntax)
    {
        Syntax = syntax;
    }
}
