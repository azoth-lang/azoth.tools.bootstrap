using System.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class FunctionPlainType : INonVoidPlainType, IMaybeFunctionPlainType
{
    public static IMaybeFunctionPlainType Create(IEnumerable<IMaybeNonVoidPlainType> parameters, IMaybePlainType @return)
    {
        if (@return is not IPlainType returnType) return IPlainType.Unknown;

        if (parameters.AsKnownFixedList() is not { } properParameters) return IPlainType.Unknown;

        return new FunctionPlainType(properParameters, returnType);
    }

    public TypeSemantics? Semantics => TypeSemantics.Reference;
    public IFixedList<INonVoidPlainType> Parameters { get; }
    public IPlainType Return { get; }
    IMaybePlainType IMaybeFunctionPlainType.Return => Return;

    public FunctionPlainType(IEnumerable<INonVoidPlainType> parameters, IPlainType returnPlainType)
    {
        Parameters = parameters.ToFixedList();
        Return = returnPlainType;
    }

    public IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType)
        => plainType;

    #region Equality
    public bool Equals(IMaybePlainType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionPlainType that
               && Parameters.Equals(that.Parameters)
               && Return.Equals(that.Return);
    }

    public override bool Equals(object? obj) => obj is IMaybePlainType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Parameters, Return);
    #endregion

    public override string ToString()
        => $"({string.Join(", ", Parameters)}) -> {Return}";
}
