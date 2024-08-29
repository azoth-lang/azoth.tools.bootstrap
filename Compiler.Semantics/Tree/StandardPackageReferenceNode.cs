using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StandardPackageReferenceNode : ChildNode, IStandardPackageReferenceNode
{
    public override IPackageReferenceSyntax Syntax { get; }

    private ValueAttribute<IPackageSymbolNode> symbolNode;
    public IPackageSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, n => Child.Attach(this, SymbolNodeAspect.PackageReference_SymbolNode(n)));

    public StandardPackageReferenceNode(IPackageReferenceSyntax syntax)
    {
        Syntax = syntax;
    }
}
