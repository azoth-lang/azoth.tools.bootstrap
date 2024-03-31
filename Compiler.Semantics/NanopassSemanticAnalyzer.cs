using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

/// <summary>
/// This is the experimental new semantic analyzer using a nanopass architecture that will probably
/// replace the old semantic analyzer.
/// </summary>
public class NanopassSemanticAnalyzer
{
    public Package Check(PackageSyntax<Package> packageSyntax)
    {
        // If there are errors from the lex and parse phase, don't continue on
        packageSyntax.Diagnostics.ThrowIfFatalErrors();

        // TODO construct the IST from the CST
        var package = SyntaxBinderPass.Build(packageSyntax);

        // TODO construct the AST from the IST
        return null!;
    }
}
