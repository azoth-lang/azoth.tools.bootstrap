using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ConversionExpressionNode : ExpressionNode, IConversionExpressionNode
{
    public override IConversionExpressionSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> referent;
    public IAmbiguousExpressionNode Referent => referent.Value;
    public ConversionOperator Operator => Syntax.Operator;
    public ITypeNode ConvertToType { get; }
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.ConversionExpression_Antetype);

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
