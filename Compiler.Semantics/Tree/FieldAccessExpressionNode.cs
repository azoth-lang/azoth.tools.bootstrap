using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FieldAccessExpressionNode : ExpressionNode, IFieldAccessExpressionNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    private Child<IExpressionNode> context;
    public IExpressionNode Context => context.Value;
    public IdentifierName FieldName { get; }
    public IFieldDeclarationNode ReferencedDeclaration { get; }
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.FieldAccessExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.FieldAccessExpression_Type);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.FieldAccessExpression_FlowStateAfter);

    public FieldAccessExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        IdentifierName fieldName,
        IFieldDeclarationNode referencedDeclaration)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        FieldName = fieldName;
        ReferencedDeclaration = referencedDeclaration;
    }
}
