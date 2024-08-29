using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class IntrinsicsPackageReferenceNode : ChildNode, IIntrinsicsPackageReferenceNode
{
    public override IPackageReferenceSyntax? Syntax => null;
    public IPackageSymbolNode SymbolNode { get; }

    /// <remarks>Not a singleton, because the parent node needs attached for each tree.</remarks>
    public IntrinsicsPackageReferenceNode()
    {
        SymbolNode = Child.Attach(this, SymbolNodeAspect.PackageReference_SymbolNode(this));
    }
}
