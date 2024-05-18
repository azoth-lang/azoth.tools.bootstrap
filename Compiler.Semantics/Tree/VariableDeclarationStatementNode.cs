using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class VariableDeclarationStatementNode : StatementNode, IVariableDeclarationStatementNode
{
    public override IVariableDeclarationStatementSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IdentifierName Name => Syntax.Name;
    public ICapabilityNode? Capability { get; }
    public ITypeNode? Type { get; }
    private Child<IAmbiguousExpressionNode?> initializer;
    public IAmbiguousExpressionNode? Initializer => initializer.Value;
    private ValueAttribute<LexicalScope> containingLexicalScope;
    public LexicalScope ContainingLexicalScope
        => containingLexicalScope.TryGetValue(out var value) ? value
            : containingLexicalScope.GetValue(InheritedContainingLexicalScope);
    private ValueAttribute<LexicalScope> lexicalScope;
    public LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopingAspect.VariableDeclarationStatement_LexicalScope);

    public VariableDeclarationStatementNode(
        IVariableDeclarationStatementSyntax syntax,
        ICapabilityNode? capability,
        ITypeNode? type,
        IAmbiguousExpressionNode? initializer)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Type = Child.Attach(this, type);
        this.initializer = Child.Create(this, initializer);
    }

    public override LexicalScope GetLexicalScope() => LexicalScope;
}
