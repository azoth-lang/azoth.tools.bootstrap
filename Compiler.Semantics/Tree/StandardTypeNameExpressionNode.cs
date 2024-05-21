using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StandardTypeNameExpressionNode : AmbiguousExpressionNode, IStandardTypeNameExpressionNode
{
    public override IStandardNameExpressionSyntax Syntax { get; }
    public StandardName Name => Syntax.Name;
    public ITypeDeclarationNode ReferencedDeclaration { get; }

    private ValueAttribute<LexicalScope> containingLexicalScope;
    public LexicalScope ContainingLexicalScope
        => containingLexicalScope.TryGetValue(out var value) ? value
            : containingLexicalScope.GetValue(InheritedContainingLexicalScope);

    public StandardTypeNameExpressionNode(
        IStandardNameExpressionSyntax syntax, ITypeDeclarationNode referencedDeclaration)
    {
        Syntax = syntax;
        ReferencedDeclaration = referencedDeclaration;
    }
}
