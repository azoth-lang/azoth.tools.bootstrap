using System;

namespace Azoth.Tools.Bootstrap.Compiler.Emit.C
{
    public class ConsoleCompilerOutput : ICompilerOutput
    {
        #region Singleton
        public static readonly ConsoleCompilerOutput Instance = new ConsoleCompilerOutput();

        private ConsoleCompilerOutput() { }
        #endregion

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
