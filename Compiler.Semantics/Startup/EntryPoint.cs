using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Startup;

internal class EntryPoint
{
    public static void Determine(PackageBuilder package)
    {
        var mainFunctions = package.NonMemberDeclarations
                                   .OfType<IFunctionDeclaration>()
                                   .Where(f => f.Symbol.Name == "main").ToList();

        // TODO warn on and remove main functions that don't have correct parameters or types
        // TODO compiler error on multiple main functions

        package.EntryPoint = mainFunctions.SingleOrDefault();
    }
}
