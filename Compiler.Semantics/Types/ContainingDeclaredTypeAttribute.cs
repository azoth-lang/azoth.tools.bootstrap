using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class ContainingDeclaredTypeAttribute
{
    public static IDeclaredUserType TypeDeclaration_InheritedContainingDeclaredType(ITypeDeclarationNode node)
        => node.DeclaredType;
}
