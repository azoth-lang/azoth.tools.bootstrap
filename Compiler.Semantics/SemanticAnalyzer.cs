using Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

public class SemanticAnalyzer
{
    public IPackageNode Check(
        IPackageFacetSyntax packageMainSyntax,
        IPackageFacetSyntax packageTestsSyntax)
    {
        // If there are errors from the lex and parse phase, don't continue on
        packageMainSyntax.Diagnostics.ThrowIfFatalErrors();
        packageTestsSyntax.Diagnostics.ThrowIfFatalErrors();

        // Build a semantic tree from the syntax tree
        var packageNode = SyntaxBinder.Bind(packageMainSyntax, packageTestsSyntax);

#if DEBUG
        // Since the tree is lazy evaluated, walk it and force evaluation of many attributes to catch bugs
        SemanticTreeValidator.Validate(packageNode);
#endif

        // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

        // If the semantic tree reports any fatal errors, don't continue on
        packageNode.Diagnostics.ThrowIfFatalErrors();

        return packageNode;
    }
}
