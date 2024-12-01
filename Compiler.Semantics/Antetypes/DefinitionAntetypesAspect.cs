using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static partial class DefinitionAntetypesAspect
{
    public static partial IOrdinaryTypeConstructor TypeDefinition_DeclaredAntetype(ITypeDefinitionNode node)
        // Types at the definition level do not depend on flow typing so it is fine to derive the
        // antetype from the type.
        => node.DeclaredType.ToTypeConstructor();

    public static partial SelfAntetype TypeDefinition_SelfAntetype(ITypeDefinitionNode node)
        => new(node.DeclaredAntetype);

    public static partial IMaybeAntetype FieldDefinition_BindingAntetype(IFieldDefinitionNode node)
        => node.TypeNode.NamedAntetype;
}
