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
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
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

        this.context = Child.Create(this, context);
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedMembers = referencedMembers.ToFixedSet();
    }

    protected override void CollectDiagnostics(DiagnosticsBuilder diagnostics)
    {
        BindingAmbiguousNamesAspect.UnknownMemberAccessExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
