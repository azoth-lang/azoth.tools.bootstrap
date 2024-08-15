using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

/// <summary>
/// Diagnostics related to the modifiers on types not matching the members.
/// </summary>
internal static class TypeModifiersAspect
{
    public static AccessModifier PackageMemberDeclaration_AccessModifier(IDefinitionNode node)
        => EntityDeclarationAccessModifier((IEntityDefinitionSyntax)node.Syntax!);

    public static AccessModifier TypeMemberDeclaration_AccessModifier(ITypeMemberDefinitionNode node)
        // Default constructors and initializers don't have syntax and are always published.
        => node.Syntax is not null ? EntityDeclarationAccessModifier(node.Syntax) : AccessModifier.Published;

    private static AccessModifier EntityDeclarationAccessModifier(IEntityDefinitionSyntax entityDefinitionSyntax)
        => entityDefinitionSyntax.AccessModifier?.ToAccessModifier() ?? AccessModifier.Private;

    public static void AbstractMethodDeclaration_ContributeDiagnostics(IAbstractMethodDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var concreteClass = !node.ContainingDeclaredType.IsAbstract;
        if (concreteClass)
            diagnostics.Add(OtherSemanticError.AbstractMethodNotInAbstractClass(node.File, node.Syntax.Span, node.Name));
    }
}
