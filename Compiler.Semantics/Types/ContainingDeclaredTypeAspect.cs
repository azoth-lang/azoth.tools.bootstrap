using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class ContainingDeclaredTypeAspect
{
    public static IDeclaredUserType TypeDeclaration_InheritedContainingDeclaredType(ITypeDefinitionNode node)
        => node.DeclaredType;
}
