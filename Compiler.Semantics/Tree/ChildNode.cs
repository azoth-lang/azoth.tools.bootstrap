using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ChildNode : SemanticNode, IChildNode
{
    public IPackageDeclarationNode Package => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());

    public abstract ISyntax? Syntax { get; }

    // TODO this should only be available in the final tree
    ISemanticNode IChildNode.Parent => (ISemanticNode)PeekParent()!;
}
