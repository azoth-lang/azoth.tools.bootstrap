using Azoth.Tools.Bootstrap.Compiler.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static class DeclarationsAntetypesAspect
{
    public static IMaybeAntetype FieldDefinition_Antetype(IFieldDefinitionNode node)
        => node.TypeNode.NamedAntetype;
}
