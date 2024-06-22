using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class SelfParameterNode : ParameterNode, ISelfParameterNode
{
    public abstract override ISelfParameterSyntax Syntax { get; }
    public bool IsLentBinding => Syntax.IsLentBinding;
    private ValueAttribute<ITypeDefinitionNode> containingTypeDeclaration;
    public ITypeDefinitionNode ContainingTypeDefinition
        => containingTypeDeclaration.TryGetValue(out var value) ? value
            : containingTypeDeclaration.GetValue(InheritedContainingTypeDefinition);
    private IDeclaredUserType? containingDeclaredType;
    private bool containingDeclaredTypeCached;
    public IDeclaredUserType ContainingDeclaredType
        => GrammarAttribute.IsCached(in containingDeclaredTypeCached) ? containingDeclaredType!
            : this.Inherited(ref containingDeclaredTypeCached, ref containingDeclaredType,
                InheritedContainingDeclaredType);
    private ValueAttribute<SelfParameterSymbol> symbol;
    public SelfParameterSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAspect.SelfParameter_Symbol);
    private ValueAttribute<IMaybeAntetype> bindingAntetype;
    public override IMaybeAntetype BindingAntetype
        => bindingAntetype.TryGetValue(out var value) ? value
            : bindingAntetype.GetValue(this, NameBindingAntetypesAspect.SelfParameter_BindingAntetype);
    private ValueAttribute<SelfParameterType> parameterType;
    public SelfParameterType ParameterType
        => parameterType.TryGetValue(out var value) ? value
            : parameterType.GetValue(this, TypeMemberDeclarationsAspect.SelfParameter_ParameterType);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : GrammarAttribute.Circular(ref flowStateAfterCached, this,
                ExpressionTypesAspect.SelfParameter_FlowStateAfter, ref flowStateAfter);
}
