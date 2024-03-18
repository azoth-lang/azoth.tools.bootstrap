using System;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
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
public class UseOfMovedValueAnalysis : IForwardDataFlowAnalysis<BindingFlags<IVariableSymbol>>
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

    public BindingFlags<IVariableSymbol> StartState()
        // All variables start without possibly having their values moved out of them
        => BindingFlags.ForVariables(declaration, symbolTree, false);

    public BindingFlags<IVariableSymbol> Assignment(
        IAssignmentExpression assignmentExpression,
        BindingFlags<IVariableSymbol> possiblyMoved)
    {
        switch (assignmentExpression.LeftOperand)
        {
            case IVariableNameExpression identifierName:
                // We are assigning into this variable so it definitely has a value now
                var symbol = identifierName.ReferencedSymbol;
                return symbol.Type is ValueType ? possiblyMoved.Set(symbol, false) : possiblyMoved;
            case IFieldAccessExpression _:
                return possiblyMoved;
            default:
                throw new NotImplementedException("Complex assignments not yet implemented");
        }
    }

    public BindingFlags<IVariableSymbol> IdentifierName(
        IVariableNameExpression nameExpression,
        BindingFlags<IVariableSymbol> possiblyMoved)
    {
        var symbol = nameExpression.ReferencedSymbol;
        if (possiblyMoved[symbol])
            diagnostics.Add(FlowTypingError.UseOfPossiblyMovedValue(file, nameExpression.Span));

        if (symbol.Type is not ValueType)
            return possiblyMoved;

        // TODO this isn't correct, but for now fields don't have proper move, borrow handling

        if (nameExpression.IsMove)
            return possiblyMoved.Set(symbol, true);

        return possiblyMoved;
    }

    public BindingFlags<IVariableSymbol> VariableDeclaration(
        IVariableDeclarationStatement variableDeclaration,
        BindingFlags<IVariableSymbol> possiblyMoved)
        // No affect on state since it should already be false
        => possiblyMoved;

    public BindingFlags<IVariableSymbol> VariableDeclaration(
        IForeachExpression foreachExpression,
        BindingFlags<IVariableSymbol> possiblyMoved)
        // No affect on state since it should already be false
        => possiblyMoved;

    public BindingFlags<IVariableSymbol> VariableDeclaration(
        IBindingPattern bindingPattern,
        BindingFlags<IVariableSymbol> possiblyMoved)
        // No affect on state since it should already be false
        => possiblyMoved;
}
