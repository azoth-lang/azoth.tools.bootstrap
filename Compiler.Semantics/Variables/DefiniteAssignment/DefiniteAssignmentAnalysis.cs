using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.DefiniteAssignment;

// TODO check definite assignment of fields in constructors
internal class DefiniteAssignmentAnalysis : IForwardDataFlowAnalysis<BindingFlags<IVariableSymbol>>
{
    private readonly IExecutableDeclaration declaration;
    private readonly ISymbolTree symbolTree;
    private readonly CodeFile file;
    private readonly Diagnostics diagnostics;

    public DefiniteAssignmentAnalysis(
        IExecutableDeclaration declaration,
        ISymbolTree symbolTree,
        Diagnostics diagnostics)
    {
        this.declaration = declaration;
        this.symbolTree = symbolTree;
        file = declaration.File;
        this.diagnostics = diagnostics;
    }

    public BindingFlags<IVariableSymbol> StartState()
    {
        var definitelyAssigned = BindingFlags.ForVariables(declaration, symbolTree, false);
        if (declaration is IInvocableDeclaration invocable)
        {
            // All parameters are assigned
            var namedParameters = invocable.Parameters.OfType<INamedParameter>();
            var parameterSymbols = namedParameters.Select(p => p.Symbol);
            definitelyAssigned = definitelyAssigned.Set(parameterSymbols, true);
        }
        return definitelyAssigned;
    }

    public BindingFlags<IVariableSymbol> Assignment(
        IAssignmentExpression assignmentExpression,
        BindingFlags<IVariableSymbol> definitelyAssigned)
    {
        return assignmentExpression.LeftOperand switch
        {
            IVariableNameExpression identifier =>
                definitelyAssigned.Set(identifier.ReferencedSymbol, true),
            IFieldAccessExpression _ => definitelyAssigned,
            _ => throw new NotImplementedException("Complex assignments not yet implemented")
        };
    }

    public BindingFlags<IVariableSymbol> IdentifierName(
        IVariableNameExpression nameExpression,
        BindingFlags<IVariableSymbol> definitelyAssigned)
    {
        if (!definitelyAssigned[nameExpression.ReferencedSymbol])
            diagnostics.Add(OtherSemanticError.VariableMayNotHaveBeenAssigned(file,
                nameExpression.Span, nameExpression.ReferencedSymbol.Name));

        return definitelyAssigned;
    }

    public BindingFlags<IVariableSymbol> VariableDeclaration(
        IVariableDeclarationStatement variableDeclaration,
        BindingFlags<IVariableSymbol> definitelyAssigned)
    {
        if (variableDeclaration.Initializer is null)
            return definitelyAssigned;
        return definitelyAssigned.Set(variableDeclaration.Symbol, true);
    }

    public BindingFlags<IVariableSymbol> VariableDeclaration(
        IForeachExpression foreachExpression,
        BindingFlags<IVariableSymbol> definitelyAssigned)
        => definitelyAssigned.Set(foreachExpression.Symbol, true);

    public BindingFlags<IVariableSymbol> VariableDeclaration(
        IBindingPattern bindingPattern,
        BindingFlags<IVariableSymbol> definitelyAssigned)
        => definitelyAssigned.Set(bindingPattern.Symbol, true);
}
