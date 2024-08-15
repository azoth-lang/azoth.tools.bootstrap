using Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

public class SemanticAnalyzer
{
    /// <summary>
    /// Whether to store the reachability graphs for each function and method.
    /// Default Value: false
    /// </summary>
    public bool SaveReachabilityGraphs { get; set; }

    public IPackageNode Check(IPackageSyntax packageSyntax)
    {
        // If there are errors from the lex and parse phase, don't continue on
        packageSyntax.Diagnostics.ThrowIfFatalErrors();

        // Assign declaration numbers before building the semantic tree so that, for now, they can
        // be used to build symbols for the old syntax tree approach.
        //DeclarationNumberAssigner.AssignIn(packageSyntax.AllEntityDeclarations);

        // Build a semantic tree from the syntax tree
        var packageNode = SyntaxBinder.Bind(packageSyntax);

#if DEBUG
        // Since the tree is lazy evaluated, walk it and force evaluation of many attributes to catch bugs
        SemanticTreeValidator.Validate(packageNode);
#endif

        // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

        // If the semantic tree reports any fatal errors, don't continue on
        packageNode.Diagnostics.ThrowIfFatalErrors();

        // If the semantic tree reports any fatal errors, don't continue on
        packageNode.Diagnostics.ThrowIfFatalErrors();

        return packageNode;
    }
}
