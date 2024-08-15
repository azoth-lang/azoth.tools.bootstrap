using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
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

    protected override IChildNode? Rewrite()
        => BindingAmbiguousNamesAspect.FunctionGroupName_Rewrite(this)
        ?? base.Rewrite();

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        BindingAmbiguousNamesAspect.FunctionGroupName_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
