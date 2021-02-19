using System.IO;
using System.Threading.Tasks;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter
{
    public class InterpreterProcess
    {
        public Task WaitForExitAsync()
        {
            // TODO
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        public TextReader StandardOutput => TextReader.Null;
        public TextReader StandardError => TextReader.Null;
        public int ExitCode => 0;
    }
}
