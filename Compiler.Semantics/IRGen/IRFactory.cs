using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IR;
using Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.IRGen
{
    internal class IRFactory
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "OO")]
        public PackageIR CreatePackage(Package package, Diagnostics diagnostics)
        {
            var builder = new IRBuilder();
            builder.Add(package.AllDeclarations);
            builder.DetermineEntryPoint(diagnostics);
            return builder.BuildPackage(package);
        }
    }
}
