using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

public abstract class DeclarationNode : CodeNode, IDeclaration
{
    public abstract override IDeclarationSyntax Syntax { get; }
}
