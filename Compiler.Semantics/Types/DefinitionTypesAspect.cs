using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class DefinitionTypesAspect
{
    public static partial IFixedList<IMaybeParameterType> InvocableDefinition_ParameterTypes(IInvocableDefinitionNode node)
        => node.Parameters.Select(p => p.ParameterType).ToFixedList();

    public static partial OrdinaryTypeConstructor TypeDefinition_Children_Broadcast_ContainingTypeConstructor(ITypeDefinitionNode node)
        => node.TypeConstructor;
}
