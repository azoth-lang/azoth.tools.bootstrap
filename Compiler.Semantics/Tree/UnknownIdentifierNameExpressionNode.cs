using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class UnknownIdentifierNameExpressionNode : UnknownStandardNameExpressionNode, IUnknownIdentifierNameExpressionNode
{
    public override IIdentifierNameExpressionSyntax Syntax { get; }
    public override IdentifierName Name => Syntax.Name;

    public UnknownIdentifierNameExpressionNode(
        IIdentifierNameExpressionSyntax syntax,
        IEnumerable<IDeclarationNode> referencedDeclarations)
        : base(referencedDeclarations)
    {
        Syntax = syntax;
    }

    protected override void CollectDiagnostics(DiagnosticsBuilder diagnostics)
    {
        BindingAmbiguousNamesAspect.UnknownIdentifierNameExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
