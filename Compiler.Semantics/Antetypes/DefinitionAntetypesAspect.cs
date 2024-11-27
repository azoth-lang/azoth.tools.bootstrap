using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static partial class DefinitionAntetypesAspect
{
    public static partial IUserDeclaredAntetype TypeDefinition_DeclaredAntetype(ITypeDefinitionNode node)
        // Types at the definition level do not depend on flow typing so it is fine to derive the
        // antetype from the type.
        => node.DeclaredType.ToAntetype();

    public static partial SelfAntetype TypeDefinition_SelfAntetype(ITypeDefinitionNode node)
        => new SelfAntetype(node.DeclaredAntetype);

    public static partial IMaybeAntetype FieldDefinition_BindingAntetype(IFieldDefinitionNode node)
        => node.TypeNode.NamedAntetype;
}
