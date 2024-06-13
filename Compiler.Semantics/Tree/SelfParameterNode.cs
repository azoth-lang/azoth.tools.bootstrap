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
    private ValueAttribute<IDeclaredUserType> containingDeclaredType;
    public IDeclaredUserType ContainingDeclaredType
        => containingDeclaredType.TryGetValue(out var value) ? value
            : containingDeclaredType.GetValue(InheritedContainingDeclaredType);
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
    private ValueAttribute<FlowState> flowStateAfter;
    public override FlowState FlowStateAfter
        => flowStateAfter.TryGetValue(out var value) ? value
            : flowStateAfter.GetValue(this, ExpressionTypesAspect.SelfParameter_FlowStateAfter);
}
