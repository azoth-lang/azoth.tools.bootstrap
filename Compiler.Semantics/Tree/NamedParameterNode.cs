using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NamedParameterNode : ParameterNode, INamedParameterNode
{
    public override INamedParameterSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public bool IsLentBinding => Syntax.IsLentBinding;
    public override IdentifierName Name => Syntax.Name;
    public int? DeclarationNumber => Syntax.DeclarationNumber.Result;
    public ITypeNode TypeNode { get; }
    private ValueAttribute<IMaybeAntetype> bindingAntetype;
    public override IMaybeAntetype BindingAntetype
        => bindingAntetype.TryGetValue(out var value) ? value
            : bindingAntetype.GetValue(this, TypeMemberDeclarationsAspect.NamedParameter_BindingAntetype);
    private DataType? bindingType;
    private bool bindingTypeCached;
    public override DataType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType, TypeMemberDeclarationsAspect.NamedParameterNode_BindingType);
    private ValueAttribute<ParameterType> parameterType;
    public ParameterType ParameterType
        => parameterType.TryGetValue(out var value) ? value
            : parameterType.GetValue(this, TypeMemberDeclarationsAspect.NamedParameter_ParameterType);
    private ValueAttribute<NamedVariableSymbol> symbol;
    public NamedVariableSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAspect.NamedParameter_Symbol);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : GrammarAttribute.Circular(ref flowStateAfterCached, this,
                ExpressionTypesAspect.NamedParameter_FlowStateAfter, ref flowStateAfter);

    public NamedParameterNode(INamedParameterSyntax syntax, ITypeNode type)
    {
        Syntax = syntax;
        TypeNode = Child.Attach(this, type);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        SymbolAspect.NamedParameter_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
