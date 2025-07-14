using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public sealed class FunctionType : NonVoidType, IMaybeFunctionType
{
    public static IMaybeFunctionType Create(IEnumerable<IMaybeParameterType> parameters, IMaybeType @return)
    {
        if (@return is not Type returnType) return Type.Unknown;

        if (parameters.AsKnownFixedList() is not { } properParameters) return Type.Unknown;

        return new FunctionType(properParameters.ToFixedList(), returnType);
    }

    public override FunctionPlainType PlainType { [DebuggerStepThrough] get; }
    IMaybeFunctionPlainType IMaybeFunctionType.PlainType => PlainType;

    public override NonVoidType? BaseType => null;

    public IFixedList<ParameterType> Parameters { [DebuggerStepThrough] get; }
    public Type Return { [DebuggerStepThrough] get; }
    IMaybeType IMaybeFunctionType.Return => Return;

    internal override GenericParameterTypeReplacements BareTypeReplacements => GenericParameterTypeReplacements.None;

    public override bool HasIndependentTypeArguments => false;

    /// <remarks>This constructor takes <paramref name="plainType"/> even though it is fully implied
    /// by the other parameters to avoid allocating duplicate <see cref="FunctionPlainType"/>s.</remarks>
    public FunctionType(FunctionPlainType plainType, IFixedList<ParameterType> parameters, Type @return)
    {
        Requires.That(plainType.Parameters.SequenceEqual(parameters.Select(p => p.PlainType)), nameof(parameters),
            "Parameters must match plain type.");
        Requires.That(plainType.Return.Equals(@return.PlainType), nameof(@return),
            "Return must match plain type.");
        PlainType = plainType;
        Parameters = parameters;
        Return = @return;
    }

    public FunctionType(IFixedList<ParameterType> parameters, Type @return)
        : this(new(parameters.Select(p => p.PlainType), @return.PlainType), parameters, @return)
    {
    }

    #region Equality
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionType otherType
               && Parameters.Equals(otherType.Parameters)
               && Return.Equals(otherType.Return);
    }

    public override int GetHashCode() => HashCode.Combine(Parameters, Return);
    #endregion

    public override string ToSourceCodeString()
        => $"({string.Join(", ", Parameters.Select(p => p.ToSourceCodeString()))}) -> {Return.ToSourceCodeString()}";

    public override string ToILString()
        => $"({string.Join(", ", Parameters.Select(p => p.ToILString()))}) -> {Return.ToILString()}";
}
