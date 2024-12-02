using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static partial class DefinitionAntetypesAspect
{
    public static partial OrdinaryTypeConstructor TypeDefinition_DeclaredAntetype(ITypeDefinitionNode node)
        // Types at the definition level do not depend on flow typing so it is fine to derive the
        // plainType from the type.
        => node.DeclaredType.ToTypeConstructor();

    public static partial SelfPlainType TypeDefinition_SelfPlainType(ITypeDefinitionNode node)
        => new(node.DeclaredAntetype);

    public static partial IMaybeAntetype FieldDefinition_BindingAntetype(IFieldDefinitionNode node)
        => node.TypeNode.NamedAntetype;
}
