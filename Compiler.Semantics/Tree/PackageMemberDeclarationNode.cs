using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class PackageMemberDeclarationNode : DeclarationNode, IPackageMemberDeclarationNode
{
    public CodeFile File => Parent.InheritedFile(this, this);
}
