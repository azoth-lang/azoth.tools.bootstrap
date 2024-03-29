using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.AST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.Shadowing;

/// <summary>
/// Enforces rules disallowing local variable shadowing
/// </summary>
internal class ShadowChecker : AbstractSyntaxWalker<BindingScope>
{
    private readonly CodeFile file;
    private readonly Diagnostics diagnostics;

    private ShadowChecker(CodeFile file, Diagnostics diagnostics)
    {
        this.file = file;
        this.diagnostics = diagnostics;
    }

    public static void Check(IEnumerable<IExecutableDeclaration> declarations, Diagnostics diagnostics)
    {
        foreach (var declaration in declarations)
            new ShadowChecker(declaration.File, diagnostics).Walk(declaration, EmptyBindingScope.Instance);
    }

    protected override void WalkNonNull(IAbstractSyntax syntax, BindingScope bindingScope)
    {
        switch (syntax)
        {
            case IConcreteInvocableDeclaration syn:
                foreach (var parameter in syn.Parameters.OfType<INamedParameter>())
                    bindingScope = new VariableBindingScope(bindingScope, parameter);
                break;
            case IFieldDeclaration syn:
                WalkChildren(syn, bindingScope);
                break;
            case IBodyOrBlock syn:
                foreach (var statement in syn.Statements)
                {
                    WalkNonNull(statement, bindingScope);
                    // Each variable declaration establishes a new binding scope
                    if (statement is IVariableDeclarationStatement variableDeclaration)
                        bindingScope = new VariableBindingScope(bindingScope, variableDeclaration);
                }
                return;
            case IVariableDeclarationStatement syn:
            {
                WalkChildren(syn, bindingScope);
                if (!bindingScope.Lookup(syn.Symbol.Name, out var binding)) return;
                if (binding.MutableBinding)
                    diagnostics.Add(OtherSemanticError.CantRebindMutableBinding(file, syn.NameSpan));
                else if (syn.Symbol.IsMutableBinding)
                    diagnostics.Add(OtherSemanticError.CantRebindAsMutableBinding(file, syn.NameSpan));
                return;
            }
            case IVariableNameExpression syn:
            {
                // This checks for cases where a variable was shadowed, but then used later
                if (!bindingScope.Lookup(syn.ReferencedSymbol.Name, out var binding)) return;
                if (binding.WasShadowedBy.Any())
                    diagnostics.Add(OtherSemanticError.CantShadow(file, binding.WasShadowedBy[^1].NameSpan, syn.Span));
                return;
            }
            case IDeclaration _:
                throw new InvalidOperationException($"Can't shadow check declaration of type {syntax.GetType().Name}");
        }

        WalkChildren(syntax, bindingScope);
    }
}
