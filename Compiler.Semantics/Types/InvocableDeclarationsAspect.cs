using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class InvocableDeclarationsAspect
{
    public static FunctionType FunctionDeclaration_Type(IFunctionDeclarationNode node)
    {
        var parameterTypes = node.Parameters.Select(p => p.Type).ToArray();
        var returnType = node.Return?.Type;
        throw new NotImplementedException();
    }
}
