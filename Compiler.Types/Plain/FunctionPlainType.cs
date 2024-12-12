using System.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class FunctionPlainType : NonVoidPlainType, IMaybeFunctionPlainType
{
    public static IMaybeFunctionPlainType Create(IEnumerable<IMaybeNonVoidPlainType> parameters, IMaybePlainType @return)
    {
        if (@return is not IPlainType returnType) return IPlainType.Unknown;

        if (parameters.AsKnownFixedList() is not { } properParameters) return IPlainType.Unknown;

        return new FunctionPlainType(properParameters, returnType);
    }

    public override TypeSemantics? Semantics => TypeSemantics.Reference;
    public IFixedList<NonVoidPlainType> Parameters { get; }
    public IPlainType Return { get; }
    IMaybePlainType IMaybeFunctionPlainType.Return => Return;

    public FunctionPlainType(IEnumerable<NonVoidPlainType> parameters, IPlainType returnPlainType)
    {
        Parameters = parameters.ToFixedList();
        Return = returnPlainType;
    }

    #region Equality
    public override bool Equals(IMaybePlainType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionPlainType that
               && Parameters.Equals(that.Parameters)
               && Return.Equals(that.Return);
    }

    public override int GetHashCode() => HashCode.Combine(Parameters, Return);
    #endregion

    public override string ToString()
        => $"({string.Join(", ", Parameters)}) -> {Return}";
}
