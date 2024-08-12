using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MethodGroupNameNode : NameExpressionNode, IMethodGroupNameNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode CurrentContext => context.UnsafeValue;
    public StandardName MethodName { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFixedSet<IStandardMethodDeclarationNode> ReferencedDeclarations { get; }
    public override IFlowState FlowStateAfter => Context.FlowStateAfter;

    public MethodGroupNameNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName methodName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IStandardMethodDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        MethodName = methodName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.MethodGroupName_ControlFlowNext(this);
}
