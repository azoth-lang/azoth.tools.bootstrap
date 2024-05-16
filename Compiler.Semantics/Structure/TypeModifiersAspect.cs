using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

/// <summary>
/// Diagnostics related to the modifiers on types not matching the members.
/// </summary>
internal static class TypeModifiersAspect
{
    public static AccessModifier PackageMemberDeclaration_AccessModifier(IDefinitionNode node)
        => EntityDeclarationAccessModifier((IEntityDeclarationSyntax)node.Syntax);

    public static AccessModifier TypeMemberDeclaration_AccessModifier(ITypeMemberDefinitionNode node)
        => EntityDeclarationAccessModifier(node.Syntax);

    private static AccessModifier EntityDeclarationAccessModifier(IEntityDeclarationSyntax entityDeclarationSyntax)
        => entityDeclarationSyntax.AccessModifier?.ToAccessModifier() ?? AccessModifier.Private;

    public static void AbstractMethodDeclaration_ContributeDiagnostics(IAbstractMethodDefinitionNode node, Diagnostics diagnostics)
    {
        var concreteClass = !node.ContainingDeclaredType.IsAbstract;
        if (concreteClass)
            diagnostics.Add(OtherSemanticError.AbstractMethodNotInAbstractClass(node.File, node.Syntax.Span, node.Name));
    }
}
