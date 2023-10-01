using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.Moves;

public class UseOfMovedValueAnalyzer : IForwardDataFlowAnalyzer<BindingFlags>
{
    #region Singleton
    public static readonly UseOfMovedValueAnalyzer Instance = new UseOfMovedValueAnalyzer();

    private UseOfMovedValueAnalyzer() { }
    #endregion

    public IForwardDataFlowAnalysis<BindingFlags> BeginAnalysis(
        IExecutableDeclaration declaration,
        ISymbolTree symbolTree,
        Diagnostics diagnostics)
    {
        return new UseOfMovedValueAnalysis(declaration, symbolTree, diagnostics);
    }
}
