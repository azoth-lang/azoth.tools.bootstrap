using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MoveExpressionNode : ExpressionNode, IMoveExpressionNode
{
    public override IMoveExpressionSyntax Syntax { get; }
    private Child<ISimpleNameNode> referent;
    public ISimpleNameNode Referent => referent.Value;
    public INameExpressionNode FinalReferent => (INameExpressionNode)referent.FinalValue;
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.MoveExpression_Antetype);

    public MoveExpressionNode(IMoveExpressionSyntax syntax, ISimpleNameNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Referent.GetFlowLexicalScope();
}
