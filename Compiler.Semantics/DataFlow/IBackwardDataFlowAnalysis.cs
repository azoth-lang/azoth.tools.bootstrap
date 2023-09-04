using Azoth.Tools.Bootstrap.Compiler.AST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;

public interface IBackwardDataFlowAnalysis<TState>
{
    TState StartState();
    TState Assignment(IAssignmentExpression assignmentExpression, TState state);
    TState IdentifierName(INameExpression nameExpression, TState state);
    TState VariableDeclaration(IVariableDeclarationStatement variableDeclaration, TState state);
    TState VariableDeclaration(IForeachExpression foreachExpression, TState state);
}
