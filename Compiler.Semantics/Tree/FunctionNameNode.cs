using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionNameNode : NameExpressionNode, IFunctionNameNode
{
    public override INameExpressionSyntax Syntax { get; }
    private RewritableChild<INameExpressionNode?> context;
    private bool contextCached;
    public INameExpressionNode? Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public INameExpressionNode? CurrentContext => context.UnsafeValue;
    public StandardName FunctionName { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFixedSet<IFunctionInvocableDeclarationNode> ReferencedDeclarations { get; }
    public IFunctionInvocableDeclarationNode? ReferencedDeclaration { get; }
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
        INameExpressionNode? context,
        StandardName functionName,
        IFixedList<ITypeNode> typeArguments,
        IFixedSet<IFunctionInvocableDeclarationNode> referencedDeclarations,
        IFunctionInvocableDeclarationNode? referencedDeclaration)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        FunctionName = functionName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations;
        ReferencedDeclaration = referencedDeclaration;
    }

    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => null;

    internal override DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => null;
}
