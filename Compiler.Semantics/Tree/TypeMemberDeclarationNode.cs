using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeMemberDeclarationNode : DeclarationNode, ITypeMemberDeclarationNode
{
    public abstract override ITypeMemberDeclarationSyntax Syntax { get; }
    public abstract override ITypeMemberSymbolNode SymbolNode { get; }
}
