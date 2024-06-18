using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ResultStatementNode : StatementNode, IResultStatementNode
{
    public override IResultStatementSyntax Syntax { get; }
    private IAmbiguousExpressionNode expression;
    public IAmbiguousExpressionNode Expression
        => GrammarAttribute.IsFinal(expression) ? expression
            : GrammarAttribute.Child(this, ref expression);
    public IExpressionNode? IntermediateExpression => Expression as IExpressionNode;
    private IMaybeAntetype? antetype;
    private bool antetypeCached;
    public IMaybeAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : GrammarAttribute.Synthetic(ref antetypeCached, this,
                ExpressionAntetypesAspect.ResultStatement_Antetype, ref antetype);
    public override IMaybeAntetype ResultAntetype => Antetype;
    private DataType? type;
    private bool typeCached;
    public DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : GrammarAttribute.Synthetic(ref typeCached, this,
                ExpressionTypesAspect.ResultStatement_Type, ref type);
    public override DataType ResultType => Type;
    public override FlowState FlowStateAfter
        => IntermediateExpression?.FlowStateAfter ?? FlowState.Empty;
    public ValueId ValueId => IntermediateExpression?.ValueId ?? default;

    public ResultStatementNode(IResultStatementSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.AttachRewritable(this, expression);
    }

    public override LexicalScope GetLexicalScope() => InheritedContainingLexicalScope();
}
