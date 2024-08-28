using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FieldParameterNode : ParameterNode, IFieldParameterNode
{
    public override IFieldParameterSyntax Syntax { get; }
    public override IdentifierName Name => Syntax.Name;
    private ValueAttribute<ITypeDefinitionNode> containingTypeDeclaration;
    public ITypeDefinitionNode ContainingTypeDefinition
        => containingTypeDeclaration.TryGetValue(out var value) ? value
            : containingTypeDeclaration.GetValue(Inherited_ContainingTypeDefinition);
    private ValueAttribute<IFieldDefinitionNode?> referencedField;
    public IFieldDefinitionNode? ReferencedField
        => referencedField.TryGetValue(out var value) ? value
            : referencedField.GetValue(this, SymbolNodeAspect.FieldParameter_ReferencedField);
    // TODO this is strange because this isn't a binding
    private IMaybeAntetype? bindingAntetype;
    private bool bindingAntetypeCached;
    public override IMaybeAntetype BindingAntetype
        => GrammarAttribute.IsCached(in bindingAntetypeCached) ? bindingAntetype!
            : this.Synthetic(ref bindingAntetypeCached, ref bindingAntetype,
                NameBindingAntetypesAspect.FieldParameter_BindingAntetype);
    // TODO this is strange because this isn't a binding
    private DataType? bindingType;
    private bool bindingTypeCached;
    public override DataType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType, TypeMemberDeclarationsAspect.FieldParameter_BindingType);
    private ValueAttribute<ParameterType> parameterType;
    public ParameterType ParameterType
        => parameterType.TryGetValue(out var value) ? value
            : parameterType.GetValue(this, TypeMemberDeclarationsAspect.FieldParameter_ParameterType);

    public override IFlowState FlowStateAfter => FlowStateBefore();

    public FieldParameterNode(IFieldParameterSyntax syntax)
    {
        Syntax = syntax;
    }
}
