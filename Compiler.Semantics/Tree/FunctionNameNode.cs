using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionNameNode : NameExpressionNode, IFunctionNameNode
{
    public override INameExpressionSyntax Syntax { get; }
    private RewritableChild<IFunctionGroupNameNode> functionGroup;
    private bool functionGroupCached;
    public IFunctionGroupNameNode FunctionGroup
        => GrammarAttribute.IsCached(in functionGroupCached) ? functionGroup.UnsafeValue
            : this.RewritableChild(ref functionGroupCached, ref functionGroup);
    public StandardName FunctionName => FunctionGroup.FunctionName;
    public IFixedList<ITypeNode> TypeArguments => FunctionGroup.TypeArguments;
    public IFunctionLikeDeclarationNode? ReferencedDeclaration { get; }
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.FunctionName_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.FunctionName_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached)
            ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.FunctionName_FlowStateAfter);

    public FunctionNameNode(
        INameExpressionSyntax syntax,
        IFunctionGroupNameNode functionGroup,
        IFunctionLikeDeclarationNode? referencedDeclaration)
    {
        Syntax = syntax;
        this.functionGroup = Child.Create(this, functionGroup);
        ReferencedDeclaration = referencedDeclaration;
    }

    public IFlowState FlowStateBefore()
        => InheritedFlowStateBefore(GrammarAttribute.CurrentInheritanceContext());

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => null;

    internal override DataType? InheritedExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => null;
}
