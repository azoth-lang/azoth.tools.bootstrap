using System;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.AST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;

internal class ForwardDataFlowAnalyzer<TState> : AbstractSyntaxWalker<bool>
    where TState : class
{
    private readonly IForwardDataFlowAnalyzer<TState> strategy;
    private readonly ISymbolTree symbolTree;
    private readonly Diagnostics diagnostics;

    public ForwardDataFlowAnalyzer(
        IForwardDataFlowAnalyzer<TState> strategy,
        ISymbolTree symbolTree,
        Diagnostics diagnostics)
    {
        this.strategy = strategy;
        this.symbolTree = symbolTree;
        this.diagnostics = diagnostics;
    }

    private IForwardDataFlowAnalysis<TState>? checker;
    private TState? currentState;

    public void Check(IExecutableDeclaration syntax) => Walk(syntax, false);

    protected override void WalkNonNull(IAbstractSyntax syntax, bool isLValue)
    {
        // TODO this doesn't handle loops correctly
        switch (syntax)
        {
            case IConcreteInvocableDeclaration exp:
                checker = strategy.BeginAnalysis(exp, symbolTree, diagnostics);
                currentState = checker.StartState();
                break;
            case IFieldDeclaration exp:
                checker = strategy.BeginAnalysis(exp, symbolTree, diagnostics);
                currentState = checker.StartState();
                break;
            case IAssignmentExpression exp:
                WalkNonNull(exp.LeftOperand, true);
                WalkNonNull(exp.RightOperand, false);
                currentState = checker!.Assignment(exp, currentState!);
                return;
            case IVariableNameExpression exp:
                if (isLValue) return; // ignore
                currentState = checker!.IdentifierName(exp, currentState!);
                return;
            case IVariableDeclarationStatement exp:
                WalkChildren(exp, false);
                currentState = checker!.VariableDeclaration(exp, currentState!);
                return;
            case IForeachExpression exp:
                WalkNonNull(exp.InExpression, isLValue);
                currentState = checker!.VariableDeclaration(exp, currentState!);
                WalkNonNull(exp.Block, isLValue);
                return;
            case IBindingPattern pat:
                currentState = checker!.VariableDeclaration(pat, currentState!);
                return;
            case IFieldAccessExpression exp:
                WalkNonNull(exp.Context, isLValue);
                // Don't walk the field name, it shouldn't be treated as a variable
                return;
            case IDeclaration _:
                throw new InvalidOperationException($"Analyze data flow of declaration of type {syntax.GetType().Name}");
        }
        WalkChildren(syntax, false);
    }
}
