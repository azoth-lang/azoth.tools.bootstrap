using System;
using System.IO;

namespace Azoth.Tools.Bootstrap.Interpreter
{
    public class InterpreterProcess
    {
        public void WaitForExit()
        {
            throw new NotImplementedException();
        }

#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
        public TextReader StandardOutput => throw new NotImplementedException();
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
        public TextReader StandardError => throw new NotImplementedException();
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
        public int ExitCode => throw new NotImplementedException();
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
    }
}
