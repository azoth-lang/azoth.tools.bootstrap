using System;
using Azoth.Tools.Bootstrap.Compiler.AST.Walkers;
using Void = Azoth.Tools.Bootstrap.Framework.Void;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter
{
    internal class InterpreterTreeWalker : AbstractSyntaxWalker<Void>
    {
        protected override void WalkNonNull(IAbstractSyntax syntax, Void arg)
        {
            throw new NotImplementedException();
        }
    }
}
