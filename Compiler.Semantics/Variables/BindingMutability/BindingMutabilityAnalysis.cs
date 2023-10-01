using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.BindingMutability;

/// <summary>
/// Uses a data flow analysis of variables and fields that are definitely unassigned to determine if
/// binding mutability is violated.
/// </summary>
public class BindingMutabilityAnalysis : IForwardDataFlowAnalysis<BindingFlags>
{
    private readonly IExecutableDeclaration declaration;
    private readonly ISymbolTree symbolTree;
    private readonly CodeFile file;
    private readonly Diagnostics diagnostics;

    public BindingMutabilityAnalysis(IExecutableDeclaration declaration, ISymbolTree symbolTree, Diagnostics diagnostics)
    {
        this.declaration = declaration;
        this.symbolTree = symbolTree;
        file = declaration.File;
        this.diagnostics = diagnostics;
    }

    public BindingFlags StartState()
    {
        // All variables start definitely unassigned
        var definitelyUnassigned = BindingFlags.ForVariables(declaration, symbolTree, true);
        if (declaration is IInvocableDeclaration invocable)
        {
            // All parameters are assigned
            var namedParameters = invocable.Parameters.OfType<INamedParameter>();
            var parameterSymbols = namedParameters.Select(p => p.Symbol);
            definitelyUnassigned = definitelyUnassigned.Set(parameterSymbols, false);
            if (invocable is IConcreteMethodDeclaration method)
            {
                var selfParameterSymbol = method.SelfParameter.Symbol;
                definitelyUnassigned = definitelyUnassigned.Set(selfParameterSymbol, false);
            }
            if (invocable is IConstructorDeclaration constructor)
            {
                var selfParameterSymbol = constructor.SelfParameter.Symbol;
                definitelyUnassigned = definitelyUnassigned.Set(selfParameterSymbol, false);
            }
        }
        return definitelyUnassigned;
    }

    public BindingFlags Assignment(
        IAssignmentExpression assignmentExpression,
        BindingFlags definitelyUnassigned)
    {
        switch (assignmentExpression.LeftOperand)
        {
            case INameExpression identifier:
                var symbol = identifier.ReferencedSymbol;
                if (!symbol.IsMutableBinding && definitelyUnassigned[symbol] == false)
                    diagnostics.Add(SemanticError.VariableMayAlreadyBeAssigned(file, identifier.Span, identifier.ReferencedSymbol.Name));
                return definitelyUnassigned.Set(symbol, false);
            case IFieldAccessExpression _:
                return definitelyUnassigned;
            default:
                throw new NotImplementedException("Complex assignments not yet implemented");
        }
    }

    public BindingFlags IdentifierName(
        INameExpression nameExpression,
        BindingFlags definitelyUnassigned)
        => definitelyUnassigned;

    public BindingFlags VariableDeclaration(
        IVariableDeclarationStatement variableDeclaration,
        BindingFlags definitelyUnassigned)
    {
        if (variableDeclaration.Initializer is null)
            return definitelyUnassigned;
        return definitelyUnassigned.Set(variableDeclaration.Symbol, false);
    }

    public BindingFlags VariableDeclaration(
        IForeachExpression foreachExpression,
        BindingFlags definitelyUnassigned)
        => definitelyUnassigned.Set(foreachExpression.Symbol, false);
}
