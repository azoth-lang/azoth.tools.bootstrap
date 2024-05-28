using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class VariableNameExpressionNode : AmbiguousNameExpressionNode, IVariableNameExpressionNode
{
    public override IIdentifierNameExpressionSyntax Syntax { get; }
    public IdentifierName Name => Syntax.Name;
    public INamedBindingNode ReferencedDeclaration { get; }

    public VariableNameExpressionNode(IIdentifierNameExpressionSyntax syntax, INamedBindingNode referencedDeclaration)
    {
        Syntax = syntax;
        ReferencedDeclaration = referencedDeclaration;
    }
}
