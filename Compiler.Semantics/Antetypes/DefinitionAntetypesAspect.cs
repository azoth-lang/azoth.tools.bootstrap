using Azoth.Tools.Bootstrap.Compiler.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static partial class DefinitionAntetypesAspect
{
    public static partial IMaybeAntetype FieldDefinition_BindingAntetype(IFieldDefinitionNode node)
        => node.TypeNode.NamedAntetype;
}
