using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeMemberDeclarationNode : DeclarationNode, ITypeMemberDeclarationNode
{
    public abstract override ITypeMemberDeclarationSyntax Syntax { get; }
}
