using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class InvocableDeclarationsAspect
{
    public static FunctionType FunctionDeclaration_Type(IFunctionDeclarationNode node)
    {
        var parameterTypes = node.Parameters.Select(p => p.Type).ToArray();
        var returnType = node.Return?.Type ?? DataType.Void;
        return new FunctionType(parameterTypes, new Return(returnType));
    }

    public static Parameter NamedParameterNode_Type(INamedParameterNode node)
        => new(node.IsLentBinding, node.TypeNode.Type);
}
