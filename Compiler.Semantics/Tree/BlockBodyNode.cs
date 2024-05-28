using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BlockBodyNode : CodeNode, IBlockBodyNode
{
    public override IBlockBodySyntax Syntax { get; }
    public IFixedList<IBodyStatementNode> Statements { get; }
    private ValueAttribute<ValueIdScope> valueIdScope;
    public ValueIdScope ValueIdScope
        => valueIdScope.TryGetValue(out var value) ? value
            : valueIdScope.GetValue(this, TypeMemberDeclarationsAspect.Body_ValueIdScope);

    public BlockBodyNode(IBlockBodySyntax syntax, IEnumerable<IBodyStatementNode> statements)
    {
        Syntax = syntax;
        Statements = ChildList.Attach(this, statements);
    }

    public LexicalScope GetContainingLexicalScope() => InheritedContainingLexicalScope();

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
        => LexicalScopingAspect.BodyOrBlock_InheritedLexicalScope(this, Statements.IndexOf(child)!.Value);

    public IParameterNode? Predecessor() => (IParameterNode?)InheritedPredecessor();
}
