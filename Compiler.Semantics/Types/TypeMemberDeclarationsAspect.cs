using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class TypeMemberDeclarationsAspect
{
    public static DataType FieldDeclaration_Type(IFieldDeclarationNode node)
        => node.TypeNode.Type;
}
