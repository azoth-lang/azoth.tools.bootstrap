using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class FunctionType : INonVoidType
{
    public FunctionPlainType PlainType { get; }
    INonVoidPlainType INonVoidType.PlainType => PlainType;
    public IFixedList<ParameterType> Parameters { get; }
    public IType Return { get; }

    public FunctionType(FunctionPlainType plainType, IFixedList<ParameterType> parameters, IType @return)
    {
        Requires.That(plainType.Parameters.SequenceEqual(parameters.Select(p => p.PlainType)), nameof(parameters),
            "Parameters must match plain type.");
        Requires.That(plainType.Return.Equals(@return.PlainType), nameof(@return),
            "Return must match plain type.");
        PlainType = plainType;
        Parameters = parameters;
        Return = @return;
    }

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString()
        => $"({string.Join(", ", Parameters.Select(p => p.ToSourceCodeString()))}) -> {Return.ToSourceCodeString()}";

    public string ToILString()
        => $"({string.Join(", ", Parameters.Select(p => p.ToILString()))}) -> {Return.ToILString()}";
}
