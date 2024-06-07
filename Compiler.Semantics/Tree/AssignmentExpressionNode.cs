using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AssignmentExpressionNode : ExpressionNode, IAssignmentExpressionNode
{
    protected override bool MayHaveRewrite => true;

    public override IAssignmentExpressionSyntax Syntax { get; }
    private Child<IAssignableExpressionNode> leftOperand;
    public IAssignableExpressionNode LeftOperand => leftOperand.Value;
    public IAssignableExpressionNode IntermediateLeftOperand => leftOperand.FinalValue;
    public AssignmentOperator Operator => Syntax.Operator;
    private Child<IAmbiguousExpressionNode> rightOperand;
    public IAmbiguousExpressionNode RightOperand => rightOperand.Value;
    public IAmbiguousExpressionNode CurrentRightOperand => rightOperand.CurrentValue;
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.AssignmentExpression_Antetype);

    public AssignmentExpressionNode(
        IAssignmentExpressionSyntax syntax,
        IAssignableExpressionNode leftOperand,
        IAmbiguousExpressionNode rightOperand)
    {
        Syntax = syntax;
        this.leftOperand = Child.Create(this, leftOperand);
        this.rightOperand = Child.Create(this, rightOperand);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope()
        => LexicalScopingAspect.AssignmentExpression_GetFlowLexicalScope(this);

    protected override IChildNode? Rewrite()
        => BindingAmbiguousNamesAspect.AssignmentExpression_Rewrite_PropertyNameLeftOperand(this);
}
