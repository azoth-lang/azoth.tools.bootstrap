using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

/// <summary>
/// Diagnostics related to the modifiers on types not matching the members.
/// </summary>
internal static partial class TypeModifiersAspect
{
    public static partial AccessModifier FunctionDefinition_AccessModifier(IFunctionDefinitionNode node)
         => EntityDefinitionAccessModifier(node.Syntax);

    public static partial AccessModifier TypeDefinition_AccessModifier(ITypeDefinitionNode node)
        => EntityDefinitionAccessModifier(node.Syntax);

    public static partial AccessModifier TypeMemberDefinition_AccessModifier(ITypeMemberDefinitionNode node)
        // Default constructors and initializers don't have syntax and are always published.
        => node.Syntax is not null ? EntityDefinitionAccessModifier(node.Syntax) : AccessModifier.Published;

    private static AccessModifier EntityDefinitionAccessModifier(IFacetMemberDefinitionSyntax entityDefinitionSyntax)
        => entityDefinitionSyntax.AccessModifier.ToAccessModifier();

    public static partial void MethodDefinition_Contribute_Diagnostics(IMethodDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (!node.IsAbstract)
            return;
        // TODO this would be better handled on the class node
        var concreteClass = !node.ContainingTypeConstructor.IsAbstract;
        if (concreteClass)
            diagnostics.Add(OtherSemanticError.AbstractMethodNotInAbstractClass(node.File, node.Syntax.Span, node.Name));
    }
}
