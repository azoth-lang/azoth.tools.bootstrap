using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StringLiteralExpressionNode : LiteralExpressionNode, IStringLiteralExpressionNode
{
    public override IStringLiteralExpressionSyntax Syntax { get; }
    public string Value => Syntax.Value;
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public override LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope, ReferenceEqualityComparer.Instance);
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.StringLiteralExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.StringLiteralExpression_Type);

    public StringLiteralExpressionNode(IStringLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics, bool contributeAttribute = true)
    {
        ExpressionTypesAspect.StringLiteralExpression_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics, contributeAttribute);
    }
}
