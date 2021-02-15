using System.IO;
using System.Threading.Tasks;

namespace Azoth.Tools.Bootstrap.Interpreter
{
    public class InterpreterProcess
    {
        public Task WaitForExitAsync()
        {
            // TODO
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
        public TextReader StandardOutput => TextReader.Null;
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
        public TextReader StandardError => TextReader.Null;
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
        public int ExitCode => 0;
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
    }
}
