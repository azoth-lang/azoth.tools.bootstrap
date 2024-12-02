using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class DefinitionAntetypesAspect
{
    public static partial OrdinaryTypeConstructor TypeDefinition_DeclaredPlainType(ITypeDefinitionNode node)
        // Types at the definition level do not depend on flow typing so it is fine to derive the
        // plainType from the type.
        => node.DeclaredType.ToTypeConstructor();

    public static partial SelfPlainType TypeDefinition_SelfPlainType(ITypeDefinitionNode node)
        => new(node.DeclaredPlainType);

    public static partial IMaybePlainType FieldDefinition_BindingPlainType(IFieldDefinitionNode node)
        => node.TypeNode.NamedPlainType;
}
