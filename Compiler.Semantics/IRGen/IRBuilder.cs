using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IR;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.IRGen
{
    internal class IRBuilder
    {
        public void Add(IEnumerable<IDeclaration> declarations)
        {
            foreach (var declaration in declarations)
                Add(declaration);
        }

        public void Add(IDeclaration declaration)
        {
            throw new NotImplementedException();
        }

        public void DetermineEntryPoint(Diagnostics diagnostics)
        {
            //var mainFunctions = declarations.OfType<FunctionIL>().Where(f => f.Symbol.Name == "main").ToList();
            //return mainFunctions.SingleOrDefault();

            // TODO warn on and remove main functions that don't have correct parameters or types
            // TODO compiler error on multiple main functions

            throw new NotImplementedException();
        }

        public PackageIR BuildPackage()
        {
            throw new NotImplementedException();
        }
    }
}
