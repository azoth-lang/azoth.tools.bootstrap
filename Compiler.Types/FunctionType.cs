using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A type for a function, closure, or method.
/// </summary>
public sealed class FunctionType : NonEmptyType, IMaybeFunctionType, INonVoidType
{
    public static IMaybeFunctionType Create(IEnumerable<ParameterType> parameters, DataType @return)
    => @return is Type returnType ? new FunctionType(parameters, returnType) : IType.Unknown;

    public FunctionType(IEnumerable<ParameterType> parameters, DataType @return)
    {
        Parameters = parameters.ToFixedList();
        Return = @return;
        IsFullyKnown = Parameters.All(p => p.IsFullyKnown) && @return.IsFullyKnown;
    }

    public int Arity => Parameters.Count;
    public IFixedList<ParameterType> Parameters { get; }
    public DataType Return { get; }

    public override bool IsFullyKnown { get; }

    #region Equality
    public override bool Equals(IMaybeExpressionType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionType otherType
               && Parameters.Equals(otherType.Parameters)
               && Return.Equals(otherType.Return);
    }

    public override int GetHashCode() => HashCode.Combine(Parameters, Return);
    #endregion

    public override IMaybeFunctionAntetype ToAntetype()
    {
        var parameters = Parameters.Select(p => p.Type.ToAntetype()).OfType<INonVoidAntetype>().ToFixedList();
        if (parameters.Count != Parameters.Count)
            // Not all parameters are known and non-void
            return IAntetype.Unknown;

        if (Return.ToAntetype() is not IAntetype returnAntetype)
            return IAntetype.Unknown;

        return new FunctionAntetype(parameters, returnAntetype);
    }
    IMaybeAntetype IMaybeType.ToAntetype() => ToAntetype();

    public override string ToSourceCodeString()
        => $"({string.Join(", ", Parameters.Select(t => t.ToSourceCodeString()))}) -> {Return.ToSourceCodeString()}";

    public override string ToILString()
        => $"({string.Join(", ", Parameters.Select(t => t.ToILString()))}) -> {Return.ToILString()}";
}
