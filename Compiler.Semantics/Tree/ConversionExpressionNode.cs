using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ConversionExpressionNode : ExpressionNode, IConversionExpressionNode
{
    public override IConversionExpressionSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> referent;
    public IAmbiguousExpressionNode Referent => referent.Value;
    public IExpressionNode FinalReferent => (IExpressionNode)referent.FinalValue;
    public ConversionOperator Operator => Syntax.Operator;
    public ITypeNode ConvertToType { get; }
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.ConversionExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : GrammarAttribute.Synthetic(ref typeCached, this,
                ExpressionTypesAspect.ConversionExpression_Type, ref type);
    public override FlowState FlowStateAfter
        => FinalReferent.FlowStateAfter;

    public ConversionExpressionNode(
        IConversionExpressionSyntax syntax,
        IAmbiguousExpressionNode referent,
        ITypeNode convertToType)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        ConvertToType = Child.Attach(this, convertToType);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Referent.GetFlowLexicalScope();
}
