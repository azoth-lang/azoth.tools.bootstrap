using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NamedParameterNode : ParameterNode, INamedParameterNode
{
    public override INamedParameterSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public bool IsLentBinding => Syntax.IsLentBinding;
    public override IdentifierName Name => Syntax.Name;
    public ITypeNode TypeNode { get; }
    private IMaybeAntetype? bindingAntetype;
    private bool bindingAntetypeCached;
    public override IMaybeAntetype BindingAntetype
        => GrammarAttribute.IsCached(in bindingAntetypeCached)
            ? bindingAntetype!
            : this.Synthetic(ref bindingAntetypeCached, ref bindingAntetype,
                TypeMemberDeclarationsAspect.NamedParameter_BindingAntetype);
    private DataType? bindingType;
    private bool bindingTypeCached;
    public override DataType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType, TypeMemberDeclarationsAspect.NamedParameter_BindingType);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                InheritedContainingLexicalScope, ReferenceEqualityComparer.Instance);
    private ValueAttribute<ParameterType> parameterType;
    public ParameterType ParameterType
        => parameterType.TryGetValue(out var value) ? value
            : parameterType.GetValue(this, TypeMemberDeclarationsAspect.NamedParameter_ParameterType);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.NamedParameter_FlowStateAfter);

    public NamedParameterNode(INamedParameterSyntax syntax, ITypeNode type)
    {
        Syntax = syntax;
        TypeNode = Child.Attach(this, type);
    }

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        SymbolAspect.NamedParameter_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    public ControlFlowSet ControlFlowFollowing()
        => InheritedControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
}
