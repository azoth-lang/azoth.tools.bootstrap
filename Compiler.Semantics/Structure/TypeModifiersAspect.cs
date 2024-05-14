using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

/// <summary>
/// Diagnostics related to the modifiers on types not matching the members.
/// </summary>
internal static class TypeModifiersAspect
{
    public static void AbstractMethodDeclaration_ContributeDiagnostics(IAbstractMethodDeclarationNode node, Diagnostics diagnostics)
    {
        var concreteClass = !node.ContainingDeclaredType.IsAbstract;
        if (concreteClass)
            diagnostics.Add(OtherSemanticError.AbstractMethodNotInAbstractClass(node.File, node.Syntax.Span, node.Name));
    }
}
