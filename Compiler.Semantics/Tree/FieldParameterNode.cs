using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
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
            : containingTypeDeclaration.GetValue(InheritedContainingTypeDefinition);
    private ValueAttribute<IFieldDefinitionNode?> referencedField;
    public IFieldDefinitionNode? ReferencedField
        => referencedField.TryGetValue(out var value) ? value
            : referencedField.GetValue(this, SymbolNodeAspect.FieldParameter_ReferencedField);
    private ValueAttribute<IMaybeAntetype> antetype;
    public override IMaybeAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, NameBindingAntetypesAspect.FieldParameter_Antetype);
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeMemberDeclarationsAspect.FieldParameter_Type);

    private ValueAttribute<Parameter> parameterType;
    public Parameter ParameterType
        => parameterType.TryGetValue(out var value) ? value
            : parameterType.GetValue(this, TypeMemberDeclarationsAspect.FieldParameter_ParameterType);
    public override FlowState FlowStateAfter => FlowStateBefore();

    public FieldParameterNode(IFieldParameterSyntax syntax)
    {
        Syntax = syntax;
    }
}
