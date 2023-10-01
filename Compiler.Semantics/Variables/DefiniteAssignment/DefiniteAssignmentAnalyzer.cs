using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables.DefiniteAssignment;

public class DefiniteAssignmentAnalyzer : IForwardDataFlowAnalyzer<BindingFlags>
{
    #region Singleton
    public static readonly DefiniteAssignmentAnalyzer Instance = new DefiniteAssignmentAnalyzer();

    private DefiniteAssignmentAnalyzer() { }
    #endregion

    public IForwardDataFlowAnalysis<BindingFlags> BeginAnalysis(
        IExecutableDeclaration declaration,
        ISymbolTree symbolTree,
        Diagnostics diagnostics)
    {
        return new DefiniteAssignmentAnalysis(declaration, symbolTree, diagnostics);
    }
}
