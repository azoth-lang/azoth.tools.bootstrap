using Azoth.Tools.Bootstrap.Compiler.Emit.C;
using Xunit.Abstractions;

namespace Azoth.Tools.Bootstrap.Tests.Conformance.Helpers
{
    public class CompilerOutputAdapter : ICompilerOutput
    {
        private readonly ITestOutputHelper testOutput;

        public CompilerOutputAdapter(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        public void WriteLine(string message)
        {
            if (message != null)
                testOutput.WriteLine(message);
        }
    }
}
