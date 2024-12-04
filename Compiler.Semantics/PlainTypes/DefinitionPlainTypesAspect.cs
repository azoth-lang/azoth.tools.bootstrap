using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class DefinitionPlainTypesAspect
{
    public static partial IMaybeFunctionPlainType ConcreteFunctionInvocableDefinition_PlainType(IConcreteFunctionInvocableDefinitionNode node)
        => FunctionPlainType.Create(node.ParameterTypes.Select(t => t.ToPlainType()), node.ReturnType.ToPlainType());

    public static partial OrdinaryTypeConstructor TypeDefinition_TypeConstructor(ITypeDefinitionNode node)
        // Types at the definition level do not depend on flow typing so it is fine to derive the
        // plainType from the type.
        => node.DeclaredType.ToTypeConstructor();

    public static partial SelfPlainType TypeDefinition_SelfPlainType(ITypeDefinitionNode node)
        => new(node.TypeConstructor);

    public static partial IMaybePlainType FieldDefinition_BindingPlainType(IFieldDefinitionNode node)
        => node.TypeNode.NamedPlainType;
}
