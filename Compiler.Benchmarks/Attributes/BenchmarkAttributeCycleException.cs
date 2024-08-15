using System.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Benchmarks.Attributes;

public class BenchmarkAttributeCycleException : Exception
{
    /// <summary>
    /// Create an exception that automatically generates a message based on the stack trace.
    /// </summary>
    public BenchmarkAttributeCycleException()
        : this(GetAttributeFrame())
    {
    }

    /// <summary>
    /// Create an exception whose message is based on the given stack frame that caused the cycle.
    /// </summary>
    public BenchmarkAttributeCycleException(StackFrame attributeFrame)
        : base(CycleMessage(attributeFrame))
    {
    }

    private static StackFrame GetAttributeFrame()
    {
        var trace = new StackTrace(3, true);
        return trace.GetFrame(0) ?? throw new InvalidOperationException("Could not get stack frame for attribute cycle message.");
    }

    private static string CycleMessage(StackFrame attributeFrame)
    {
        var attributeMethod = attributeFrame.GetMethod();
        var typeName = attributeMethod?.DeclaringType?.GetFriendlyName();
        var memberName = attributeMethod?.GetProperty()?.Name ?? attributeMethod?.Name;
        return $"Cyclic dependency detected while computing `{typeName}.{memberName}`.";
    }
}
