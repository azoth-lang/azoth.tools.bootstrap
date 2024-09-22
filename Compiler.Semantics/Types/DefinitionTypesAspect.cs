using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class DefinitionTypesAspect
{
    public static partial IFixedList<IMaybeParameterType> InvocableDefinition_ParameterTypes(IInvocableDefinitionNode node)
        => node.Parameters.Select(p => p.ParameterType).ToFixedList();

    public static partial IDeclaredUserType TypeDefinition_Children_Broadcast_ContainingDeclaredType(ITypeDefinitionNode node)
        => node.DeclaredType;
}
