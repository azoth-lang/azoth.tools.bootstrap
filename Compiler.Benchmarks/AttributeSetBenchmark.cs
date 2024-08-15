using Azoth.Tools.Bootstrap.Compiler.Benchmarks.Attributes;
using BenchmarkDotNet.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.Benchmarks;

public class AttributeSetBenchmark
{
    private BenchmarkValueAttribute<object> attribute;
    /// <summary>
    /// Cache the instance to avoid creating a new one every time.
    /// </summary>
    private static readonly object Instance = new object();

    private readonly object value = new object();

    [Benchmark]
    public object DirectSetValueSimpleStatic()
    {
        attribute.Reset();
        return attribute.GetValueSimple(this, ComputeValueStatic);
    }

    [Benchmark]
    public object DirectSetValueSimpleInstance()
    {
        attribute.Reset();
        return attribute.GetValueSimple(ComputeValueInstance);
    }

    [Benchmark]
    public object DirectSetValueSimpleLambda()
    {
        attribute.Reset();
        return attribute.GetValueSimple(this, ComputeValueStatic);
    }

    [Benchmark]
    public object DirectSetValueSmartStatic()
    {
        attribute.Reset();
        return attribute.GetValueSmart(this, ComputeValueStatic);
    }

    [Benchmark]
    public object DirectSetValueSmartInstance()
    {
        attribute.Reset();
        return attribute.GetValueSmart(ComputeValueInstance);
    }

    [Benchmark]
    public object DirectSetValueSmartLambda()
    {
        attribute.Reset();
        return attribute.GetValueSmart(this, ComputeValueStatic);
    }

    [Benchmark]
    public object TrySetValue()
    {
        attribute.Reset();
        return attribute.TryGetValue(out var value) ? value : attribute.GetValueSimple(this, ComputeValueStatic);
    }


    private static object ComputeValueStatic(AttributeSetBenchmark _) => Instance;

    /// <remarks>Non-static for performance testing.</remarks>
    private object ComputeValueInstance() => value;
}
