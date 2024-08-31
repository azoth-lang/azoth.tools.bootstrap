using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

/// <summary>
/// Diagnostics related to the modifiers on types not matching the members.
/// </summary>
internal static partial class TypeModifiersAspect
{
    public static AccessModifier PackageMemberDefinition_AccessModifier(IPackageMemberDefinitionNode node)
        => EntityDeclarationAccessModifier((IEntityDefinitionSyntax)node.Syntax!);

    public static partial AccessModifier FunctionDefinition_AccessModifier(IFunctionDefinitionNode node)
         => EntityDeclarationAccessModifier(node.Syntax);

    public static partial AccessModifier TypeDefinition_AccessModifier(ITypeDefinitionNode node)
        => EntityDeclarationAccessModifier(node.Syntax);

    public static partial AccessModifier TypeMemberDefinition_AccessModifier(ITypeMemberDefinitionNode node)
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
