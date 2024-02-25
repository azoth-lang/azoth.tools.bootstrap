using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Azoth.Tools.Bootstrap.Tests.Unit;

[Trait("Category", "Characterization")]
public class DotNetFrameworkTests
{
    private readonly ITestOutputHelper output;

    public DotNetFrameworkTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void ClosureTypes()
    {
        var lambda1 = GetMethodInfo(x => 1);
        var lambda2 = GetMethodInfo(x => 2);
        output.WriteLine(lambda1.Name);

        output.WriteLine(lambda2.Name);
        Assert.NotEqual(lambda1, lambda2);
    }

    private static MethodInfo GetMethodInfo(Func<int, int> f)
    {
        return f.Method;
    }

    [Fact]
    public void LazyCycleThrowsInvalidOperation()
    {
        Lazy<int>? l2 = null;
        var l1 = new Lazy<int>(() => l2!.Value);
        l2 = new Lazy<int>(() => l1.Value);
        Assert.Throws<InvalidOperationException>(() => l1.Value);
    }

    [Fact]
    public async Task TaskSelfReferenceDeadlocks()
    {
        var task = Task.FromResult(1);
        task = GetValueAsync(() => task);

        var delayTask = Task.Delay((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
        var completedTask = await Task.WhenAny(task, delayTask);
        Assert.Equal(delayTask, completedTask);
    }

    [Fact]
    public async Task TaskCycleDeadlocks()
    {
        var t2 = Task.FromResult(2);
        var t1 = GetValueAsync(() => t2);
        t2 = GetValueAsync(() => t1);

        var delayTask = Task.Delay((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
        var completedTask = await Task.WhenAny(t1, delayTask);
        Assert.Equal(delayTask, completedTask);
    }

    private static async Task<int> GetValueAsync(Func<Task<int>> task)
    {
        await Task.Yield();
        return await task().ConfigureAwait(false);
    }

    private string testField = "old";

    [Fact]
    public void CanPassFieldByRef()
    {
        TakeByRef(ref testField);
    }

    private static void TakeByRef(ref string x)
    {
        x = "new";
    }

    [Fact]
    public void CanPassArrayElementByRef()
    {
        var strings = new string[] { "old1", "old2" };
        TakeByRef(ref strings[0]);
    }

    [Fact]
    public void BecomeNonNull()
    {
        var x = new Ref<string?>("Hello!");

        var z = x.HasValue();
        Assert.Equal("Hello!", z.Value);
    }

    [Fact]
    public void BoxedStructThisPassedByRef()
    {
        IIncrementable x = new MutableStruct();
        x.Increment();
        Assert.Equal(1, x.Value);
    }

    [Fact]
    public void CanObserveBoxedStructStateChange()
    {

        IIncrementable x = new MutableStruct();
        x.Observe((in MutableStruct y) =>
        {
            Assert.Equal(0, y.Value);
            x.Increment();
            Assert.Equal(1, x.Value);
        });
    }

    private interface IIncrementable
    {
        public int Value { get; }
        void Increment();
        void Observe(ObserveDelegate observer);
    }

    private delegate void ObserveDelegate(in MutableStruct x);

    private struct MutableStruct : IIncrementable
    {
        public MutableStruct()
        {
        }

        public int Value { get; set; } = 0;

        public void Increment()
        {
            Value++;
        }

        public readonly void Observe(ObserveDelegate observer)
        {
            observer(in this);
        }

        public void PassThis()
        {
            ref MutableStruct x = ref this;
            IncrementByRef(ref this);
        }
    }

    private static void IncrementByRef(ref MutableStruct x)
    {
        x.Increment();
        ref MutableStruct y = ref x;
    }
}

public class Ref<T>
    where T : class?
{
    public T Value { get; }

    public Ref(T value)
    {
        Value = value;
    }

}

public static class RefExtensions
{
    public static Ref<T> HasValue<T>(this Ref<T?> value)
        where T : class
    {
        if (value.Value is null)
            throw new InvalidOperationException();
        return value!;
    }
}
