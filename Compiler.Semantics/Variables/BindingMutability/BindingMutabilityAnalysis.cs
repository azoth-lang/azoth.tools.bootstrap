using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
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
        var definitelyUnassigned = BindingFlags.ForVariablesAndFields(declaration, symbolTree, true);
        if (declaration is IInvocableDeclaration invocable)
        {
            // All parameters are assigned
            var namedParameters = invocable.Parameters.OfType<INamedParameter>();
            var parameterSymbols = namedParameters.Select(p => p.Symbol);
            definitelyUnassigned = definitelyUnassigned.Set(parameterSymbols, false);

            if (invocable is IConcreteMethodDeclaration method)
            {
                // self parameter is assigned
                var selfParameterSymbol = method.SelfParameter.Symbol;
                definitelyUnassigned = definitelyUnassigned.Set(selfParameterSymbol, false);
                // fields are assigned
                var typeSymbol = method.DeclaringType.Symbol;
                var fieldSymbols = symbolTree.Children(typeSymbol).OfType<FieldSymbol>();
                definitelyUnassigned = definitelyUnassigned.Set(fieldSymbols, false);
            }
            if (invocable is IConstructorDeclaration constructor)
            {
                // self parameter is assigned
                var selfParameterSymbol = constructor.SelfParameter.Symbol;
                definitelyUnassigned = definitelyUnassigned.Set(selfParameterSymbol, false);
                // fields are not assigned
                // TODO does this depend on field initializers
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
            {
                var symbol = identifier.ReferencedSymbol;
                if (!symbol.IsMutableBinding && definitelyUnassigned[symbol] == false)
                    diagnostics.Add(SemanticError.MayAlreadyBeAssigned(file, identifier.Span,
                        symbol.Name));
                return definitelyUnassigned.Set(symbol, false);
            }
            case IFieldAccessExpression fieldAccess:
                if (fieldAccess.Context is ISelfExpression)
                {
                    var symbol = fieldAccess.ReferencedSymbol;
                    if (!symbol.IsMutableBinding && definitelyUnassigned[symbol] == false)
                        diagnostics.Add(SemanticError.MayAlreadyBeAssigned(file, fieldAccess.Span, symbol.Name));
                    definitelyUnassigned = definitelyUnassigned.Set(symbol, false);
                }
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
