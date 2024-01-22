using System;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using ExhaustiveMatching;
using ValueType = Azoth.Tools.Bootstrap.Compiler.Types.ValueType;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.Moves;

/// <summary>
/// Uses a data flow analysis of variables that may have their value moved
/// out of them to check for use of possibly moved value.
///
/// The variable flags used by this checker indicate that a variable may have
/// its value moved. Variables not yet declared or assigned vacuously haven't
/// been moved from.
/// </summary>
public class UseOfMovedValueAnalysis : IForwardDataFlowAnalysis<BindingFlags>
{
    private readonly IExecutableDeclaration declaration;
    private readonly ISymbolTree symbolTree;
    private readonly CodeFile file;
    private readonly Diagnostics diagnostics;

    public UseOfMovedValueAnalysis(
        IExecutableDeclaration declaration,
        ISymbolTree symbolTree,
        Diagnostics diagnostics)
    {
        this.declaration = declaration;
        this.symbolTree = symbolTree;
        file = declaration.File;
        this.diagnostics = diagnostics;
    }

    public BindingFlags StartState()
        // All variables start without possibly having their values moved out of them
        => BindingFlags.ForVariables(declaration, symbolTree, false);

    public BindingFlags Assignment(
        IAssignmentExpression assignmentExpression,
        BindingFlags possiblyMoved)
    {
        switch (assignmentExpression.LeftOperand)
        {
            case IVariableNameExpression identifierName:
                // We are assigning into this variable so it definitely has a value now
                var symbol = identifierName.ReferencedSymbol;
                return symbol.DataType is ValueType ? possiblyMoved.Set(symbol, false) : possiblyMoved;
            case IFieldAccessExpression _:
                return possiblyMoved;
            default:
                throw new NotImplementedException("Complex assignments not yet implemented");
        }
    }

    public BindingFlags IdentifierName(
        IVariableNameExpression nameExpression,
        BindingFlags possiblyMoved)
    {
        var symbol = nameExpression.ReferencedSymbol;
        if (possiblyMoved[symbol] == true)
            diagnostics.Add(FlowTypingError.UseOfPossiblyMovedValue(file, nameExpression.Span));

        if (symbol.DataType is not ValueType)
            return possiblyMoved;

        var valueSemantics = nameExpression.Semantics;
        // TODO this isn't correct, but for now fields don't have proper move, borrow handling
        //?? nameExpression.Type.Assigned().OldValueSemantics;
        switch (valueSemantics)
        {
            case ExpressionSemantics.MoveValue:
            case ExpressionSemantics.IsolatedReference:
                return possiblyMoved.Set(symbol, true);
            case ExpressionSemantics.CopyValue:
            case ExpressionSemantics.MutableReference:
            case ExpressionSemantics.ReadOnlyReference:
            case ExpressionSemantics.ConstReference:
            case ExpressionSemantics.IdReference:
            case ExpressionSemantics.Void:
            case ExpressionSemantics.Never:
            case ExpressionSemantics.CreateReference:
                // If it were move or copy, that would have been set to the ExpressionSemantics
                // Not moving value
                return possiblyMoved;
            default:
                throw ExhaustiveMatch.Failed(valueSemantics);
        }
    }

    public BindingFlags VariableDeclaration(
        IVariableDeclarationStatement variableDeclaration,
        BindingFlags possiblyMoved)
        // No affect on state since it should already be false
        => possiblyMoved;

    public BindingFlags VariableDeclaration(
        IForeachExpression foreachExpression,
        BindingFlags possiblyMoved)
        // No affect on state since it should already be false
        => possiblyMoved;

    public BindingFlags VariableDeclaration(
        IBindingPattern bindingPattern,
        BindingFlags possiblyMoved)
        // No affect on state since it should already be false
        => possiblyMoved;
}
