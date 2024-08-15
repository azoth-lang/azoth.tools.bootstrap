using BenchmarkDotNet.Running;

namespace Azoth.Tools.Bootstrap.Compiler.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run(typeof(Program).Assembly, args: args);
    }
}
