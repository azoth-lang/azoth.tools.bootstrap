using System;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Liveness;

public class LivenessAnalysis : IBackwardDataFlowAnalysis<BindingFlags<IVariableSymbol>>
{
    private readonly IExecutableDeclaration declaration;
    private readonly ISymbolTree symbolTree;

    public LivenessAnalysis(IExecutableDeclaration declaration, ISymbolTree symbolTree)
    {
        this.declaration = declaration;
        this.symbolTree = symbolTree;
    }

    public BindingFlags<IVariableSymbol> StartState()
        => BindingFlags.ForVariables(declaration, symbolTree, false);

    public BindingFlags<IVariableSymbol> Assignment(
        IAssignmentExpression assignmentExpression,
        BindingFlags<IVariableSymbol> liveVariables)
    {
        switch (assignmentExpression.LeftOperand)
        {
            case IVariableNameExpression identifier:
                var symbol = identifier.ReferencedSymbol;
                var isLifeAfter = liveVariables[symbol]
                                  ?? throw new Exception($"No liveness data for variable {symbol}");
                identifier.VariableIsLiveAfter.Fulfill(isLifeAfter);
                return liveVariables.Set(symbol, false);
            case IFieldAccessExpression _:
                return liveVariables;
            default:
                throw new NotImplementedException("Complex assignments not yet implemented");
        }
    }

    public BindingFlags<IVariableSymbol> IdentifierName(
        IVariableNameExpression nameExpression,
        BindingFlags<IVariableSymbol> liveVariables)
    {
        SetLiveness(nameExpression.ReferencedSymbol, nameExpression.VariableIsLiveAfter, liveVariables);
        return liveVariables.Set(nameExpression.ReferencedSymbol, true);
    }

    public BindingFlags<IVariableSymbol> VariableDeclaration(
        IVariableDeclarationStatement variableDeclaration,
        BindingFlags<IVariableSymbol> liveVariables)
    {
        SetLiveness(variableDeclaration.Symbol, variableDeclaration.VariableIsLiveAfter, liveVariables);
        return liveVariables.Set(variableDeclaration.Symbol, false);
    }

    public BindingFlags<IVariableSymbol> VariableDeclaration(
        IForeachExpression foreachExpression,
        BindingFlags<IVariableSymbol> liveVariables)
    {
        SetLiveness(foreachExpression.Symbol, foreachExpression.VariableIsLiveAfterAssignment, liveVariables);
        return liveVariables.Set(foreachExpression.Symbol, false);
    }

    public BindingFlags<IVariableSymbol> VariableDeclaration(
        IBindingPattern bindingPattern,
        BindingFlags<IVariableSymbol> liveVariables)
    {
        SetLiveness(bindingPattern.Symbol, bindingPattern.VariableIsLiveAfter, liveVariables);
        return liveVariables.Set(bindingPattern.Symbol, false);
    }

    private static void SetLiveness(
        IVariableSymbol symbol,
        Promise<bool> promise,
        BindingFlags<IVariableSymbol> liveVariables)
    {
        var isLiveAfter = liveVariables[symbol]
                          ?? throw new Exception($"No liveness data for variable {symbol}");

        promise.Fulfill(isLiveAfter);
    }
}
