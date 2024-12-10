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

    /// <remarks>This constructor takes <paramref name="plainType"/> even though it is fully implied
    /// by the other parameters to avoid allocating duplicate <see cref="FunctionPlainType"/>s.</remarks>
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

    public FunctionType(IFixedList<ParameterType> parameters, IType @return)
        : this(new(parameters.Select(p => p.PlainType), @return.PlainType), parameters, @return)
    {
    }

    #region Equality
    public bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionType otherType
               && Parameters.Equals(otherType.Parameters)
               && Return.Equals(otherType.Return);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is FunctionType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Parameters, Return);
    #endregion

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString()
        => $"({string.Join(", ", Parameters.Select(p => p.ToSourceCodeString()))}) -> {Return.ToSourceCodeString()}";

    public string ToILString()
        => $"({string.Join(", ", Parameters.Select(p => p.ToILString()))}) -> {Return.ToILString()}";
}
