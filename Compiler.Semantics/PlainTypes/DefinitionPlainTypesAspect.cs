using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class DefinitionPlainTypesAspect
{
    #region Definitions
    public static partial IFixedList<IMaybeNonVoidPlainType> InvocableDefinition_ParameterPlainTypes(IInvocableDefinitionNode node)
        => node.Parameters.Select(p => p.BindingPlainType).ToFixedList();

    public static partial IMaybeFunctionPlainType FunctionInvocableDefinition_PlainType(IFunctionInvocableDefinitionNode node)
        => FunctionPlainType.Create(node.ParameterPlainTypes, node.ReturnPlainType);
    #endregion

    #region Member Definitions
    public static partial IMaybeNonVoidPlainType FieldDefinition_BindingPlainType(IFieldDefinitionNode node)
        => node.TypeNode.NamedPlainType.ToNonVoid();
    #endregion
}
