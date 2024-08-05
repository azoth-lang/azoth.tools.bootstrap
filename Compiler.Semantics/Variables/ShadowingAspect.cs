using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

internal static class ShadowingAspect
{
    public static void VariableBinding_ContributeDiagnostics(IVariableBindingNode node, Diagnostics diagnostics)
    {
        if (node.ContainingLexicalScope.Lookup(node.Name).TrySingle() is INamedBindingNode shadowedDeclaration)
        {
            if (shadowedDeclaration.IsMutableBinding)
                diagnostics.Add(OtherSemanticError.CantRebindMutableBinding(node.File, node.Syntax.NameSpan));
            else if (node.IsMutableBinding)
                diagnostics.Add(OtherSemanticError.CantRebindAsMutableBinding(node.File, node.Syntax.NameSpan));
        }
    }
}
