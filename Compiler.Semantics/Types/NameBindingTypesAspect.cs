using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class NameBindingTypesAspect
{
    public static DataType VariableDeclarationStatement_BindingType(IVariableDeclarationStatementNode node)
        // TODO account for node.Capability
        => node.Type?.NamedType ?? node.FinalInitializer?.Type.ToNonConstValueType() ?? DataType.Unknown;
}
