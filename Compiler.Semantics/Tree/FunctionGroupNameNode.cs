using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionGroupNameNode : NameExpressionNode, IFunctionGroupNameNode
{
    protected override bool MayHaveRewrite => true;

    public override INameExpressionSyntax Syntax { get; }
    public INameExpressionNode? Context { get; }
    public StandardName FunctionName { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFixedSet<IFunctionLikeDeclarationNode> ReferencedDeclarations { get; }

    public FunctionGroupNameNode(
        INameExpressionSyntax syntax,
        INameExpressionNode? context,
        StandardName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IFunctionLikeDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        FunctionName = functionName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
        Requires.That(!ReferencedDeclarations.IsEmpty, nameof(referencedDeclarations),
            "Must be at least one referenced declaration");
    }

    protected override IChildTreeNode Rewrite()
        => BindingAmbiguousNamesAspect.FunctionGroupName_Rewrite(this)
        ?? base.Rewrite();

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        BindingAmbiguousNamesAspect.FunctionGroupName_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics);
    }
}
