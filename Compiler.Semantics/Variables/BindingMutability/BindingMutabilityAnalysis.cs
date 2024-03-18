using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.BindingMutability;

/// <summary>
/// Uses a data flow analysis of variables and fields that are definitely unassigned to determine if
/// binding mutability is violated.
/// </summary>
public class BindingMutabilityAnalysis : IForwardDataFlowAnalysis<BindingFlags<IBindingSymbol>>
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

    public BindingFlags<IBindingSymbol> StartState()
    {
        // All variables start definitely unassigned
        var definitelyUnassigned = BindingFlags.ForVariablesAndFields(declaration, symbolTree, true);
        if (declaration is IInvocableDeclaration invocable)
        {
            // All named parameters are assigned
            var namedParameters = invocable.Parameters.OfType<INamedParameter>();
            var parameterSymbols = namedParameters.Select(p => p.Symbol);
            definitelyUnassigned = definitelyUnassigned.Set(parameterSymbols, false);
            // All field parameters assign their fields
            var fieldParameters = invocable.Parameters.OfType<IFieldParameter>();
            var fieldSymbols = fieldParameters.Select(p => p.ReferencedSymbol);
            definitelyUnassigned = definitelyUnassigned.Set(fieldSymbols, false);

            switch (invocable)
            {
                default:
                    throw ExhaustiveMatch.Failed(invocable);
                case IConcreteMethodDeclaration method:
                {
                    // self parameter is assigned
                    var selfParameterSymbol = method.SelfParameter.Symbol;
                    definitelyUnassigned = definitelyUnassigned.Set(selfParameterSymbol, false);
                    // fields are assigned
                    var typeSymbol = method.DeclaringType.Symbol;
                    fieldSymbols = symbolTree.GetChildrenOf(typeSymbol).OfType<FieldSymbol>();
                    definitelyUnassigned = definitelyUnassigned.Set(fieldSymbols, false);
                    break;
                }
                case IConstructorDeclaration constructor:
                {
                    // self parameter is assigned
                    var selfParameterSymbol = constructor.SelfParameter.Symbol;
                    definitelyUnassigned = definitelyUnassigned.Set(selfParameterSymbol, false);
                    // fields are not assigned
                    // TODO does this depend on field initializers
                    break;
                }
                case IInitializerDeclaration initializer:
                {
                    // self parameter is assigned
                    var selfParameterSymbol = initializer.SelfParameter.Symbol;
                    definitelyUnassigned = definitelyUnassigned.Set(selfParameterSymbol, false);
                    // fields are not assigned
                    // TODO does this depend on field initializers
                    break;
                }
                case IConcreteFunctionInvocableDeclaration _:
                    // No extra work to do
                    break;
            }
        }
        return definitelyUnassigned;
    }

    public BindingFlags<IBindingSymbol> Assignment(
        IAssignmentExpression assignmentExpression,
        BindingFlags<IBindingSymbol> definitelyUnassigned)
    {
        switch (assignmentExpression.LeftOperand)
        {
            case IVariableNameExpression identifier:
            {
                var symbol = identifier.ReferencedSymbol;
                if (!symbol.IsMutableBinding && !definitelyUnassigned[symbol])
                    diagnostics.Add(OtherSemanticError.MayAlreadyBeAssigned(file, identifier.Span,
                        symbol.Name));
                return definitelyUnassigned.Set(symbol, false);
            }
            case IFieldAccessExpression fieldAccess:
                if (fieldAccess.Context is ISelfExpression)
                {
                    var symbol = fieldAccess.ReferencedSymbol;
                    if (!symbol.IsMutableBinding && !definitelyUnassigned[symbol])
                        diagnostics.Add(OtherSemanticError.MayAlreadyBeAssigned(file, fieldAccess.Span, symbol.Name));
                    definitelyUnassigned = definitelyUnassigned.Set(symbol, false);
                }
                return definitelyUnassigned;
            default:
                throw new NotImplementedException("Complex assignments not yet implemented");
        }
    }

    public BindingFlags<IBindingSymbol> IdentifierName(
        IVariableNameExpression nameExpression,
        BindingFlags<IBindingSymbol> definitelyUnassigned)
        => definitelyUnassigned;

    public BindingFlags<IBindingSymbol> VariableDeclaration(
        IVariableDeclarationStatement variableDeclaration,
        BindingFlags<IBindingSymbol> definitelyUnassigned)
    {
        if (variableDeclaration.Initializer is null)
            return definitelyUnassigned;
        return definitelyUnassigned.Set(variableDeclaration.Symbol, false);
    }

    public BindingFlags<IBindingSymbol> VariableDeclaration(
        IForeachExpression foreachExpression,
        BindingFlags<IBindingSymbol> definitelyUnassigned)
        => definitelyUnassigned.Set(foreachExpression.Symbol, false);

    public BindingFlags<IBindingSymbol> VariableDeclaration(
        IBindingPattern bindingPattern,
        BindingFlags<IBindingSymbol> definitelyUnassigned)
        => definitelyUnassigned.Set(bindingPattern.Symbol, false);
}
