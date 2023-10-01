using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.BindingMutability;

public class BindingMutabilityAnalyzer : IForwardDataFlowAnalyzer<VariableFlags>
{
    #region Singleton
    public static readonly BindingMutabilityAnalyzer Instance = new BindingMutabilityAnalyzer();

    private BindingMutabilityAnalyzer() { }
    #endregion

    public IForwardDataFlowAnalysis<VariableFlags> BeginAnalysis(
        IExecutableDeclaration declaration,
        ISymbolTree symbolTree,
        Diagnostics diagnostics)
        => new BindingMutabilityAnalysis(declaration, symbolTree, diagnostics);
}
