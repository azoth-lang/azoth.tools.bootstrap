using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

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

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        BindingAmbiguousNamesAspect.UnknownIdentifierNameExpression_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics);
    }
}
