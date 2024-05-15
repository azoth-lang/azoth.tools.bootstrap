using Azoth.Tools.Bootstrap.Compiler.Benchmarks.Attributes;
using BenchmarkDotNet.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.Benchmarks;

/// <summary>
/// Benchmark the time it takes to read an already set attribute.
/// </summary>
public class AttributeReadBenchmark
{
    private BenchmarkValueAttribute<object> attribute;
    private readonly object value = new object();

    public AttributeReadBenchmark()
    {
        attribute.GetValueSimple(this, ComputeValueStatic);
    }

    [Benchmark]
    public object DirectGetValueSimpleStatic() => attribute.GetValueSimple(this, ComputeValueStatic);

    [Benchmark]
    public object DirectGetValueSimpleInstance() => attribute.GetValueSimple(ComputeValueInstance);

    [Benchmark]
    public object DirectGetValueSimpleLambda() => attribute.GetValueSimple(this, ComputeValueStatic);

    [Benchmark]
    public object DirectGetValueSmartStatic() => attribute.GetValueSmart(this, ComputeValueStatic);

    [Benchmark]
    public object DirectGetValueSmartInstance() => attribute.GetValueSmart(ComputeValueInstance);

    [Benchmark]
    public object DirectGetValueSmartLambda() => attribute.GetValueSmart(this, ComputeValueStatic);

    [Benchmark]
    public object TryGetValue()
        => attribute.TryGetValue(out var value) ? value : attribute.GetValueSimple(this, ComputeValueStatic);

    private static object ComputeValueStatic(AttributeReadBenchmark _) => new object();

    /// <remarks>Non-static for performance testing.</remarks>
    private object ComputeValueInstance() => value;
}
