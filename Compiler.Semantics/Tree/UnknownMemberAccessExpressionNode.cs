using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class UnknownMemberAccessExpressionNode : UnknownNameExpressionNode, IUnknownMemberAccessExpressionNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    public IExpressionNode Context { get; }
    public StandardName MemberName => Syntax.MemberName;
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFixedSet<IDeclarationNode> ReferencedMembers { get; }

    public UnknownMemberAccessExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IDeclarationNode> referencedMembers)
    {
        Syntax = syntax;

        Context = Child.Attach(this, context);
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedMembers = referencedMembers.ToFixedSet();
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        BindingAmbiguousNamesAspect.UnknownMemberAccessExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    internal override IFlowNode InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant == Context)
            return Predecessor();
        return base.InheritedPredecessor(child, descendant);
    }
}
