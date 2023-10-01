using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Liveness;

public class LivenessAnalyzer : IBackwardDataFlowAnalyzer<BindingFlags>
{
    #region Singleton
    public static readonly LivenessAnalyzer Instance = new();

    private LivenessAnalyzer() { }
    #endregion

    public IBackwardDataFlowAnalysis<BindingFlags> BeginAnalysis(
        IExecutableDeclaration declaration,
        ISymbolTree symbolTree,
        Diagnostics _)
        => new LivenessAnalysis(declaration, symbolTree);
}
