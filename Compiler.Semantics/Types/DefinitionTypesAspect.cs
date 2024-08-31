using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class DefinitionTypesAspect
{
    public static partial IDeclaredUserType TypeDefinition_Children_Broadcast_ContainingDeclaredType(ITypeDefinitionNode node)
        => node.DeclaredType;
}
