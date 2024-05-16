using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class VariableDeclarationStatementNode : CodeNode, IVariableDeclarationStatementNode
{
    public override IVariableDeclarationStatementSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IdentifierName Name => Syntax.Name;
    public ICapabilityNode? Capability { get; }
    public ITypeNode? Type { get; }
    private Child<IUntypedExpressionNode?> initializer;
    public IUntypedExpressionNode? Initializer => initializer.Value;

    public VariableDeclarationStatementNode(
        IVariableDeclarationStatementSyntax syntax,
        ICapabilityNode? capability,
        ITypeNode? type,
        IUntypedExpressionNode? initializer)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Type = Child.Attach(this, type);
        this.initializer = Child.Create(this, initializer);
    }
}
