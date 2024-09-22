using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A type for a function, closure, or method.
/// </summary>
public sealed class FunctionType : NonEmptyType, IMaybeFunctionType, INonVoidType
{
    public static IMaybeFunctionType Create(IEnumerable<IMaybeParameterType> parameters, IMaybeType @return)
    {
        if (@return is not IType returnType) return IType.Unknown;

        if (parameters.AsKnownFixedList() is not { } properParameters) return IType.Unknown;

        return new FunctionType(properParameters.ToFixedList(), returnType);
    }

    public FunctionType(IEnumerable<ParameterType> parameters, IType @return)
    {
        Parameters = parameters.ToFixedList();
        Return = @return;
        IsFullyKnown = Parameters.All(p => p.IsFullyKnown) && @return.IsFullyKnown;
    }

    public int Arity => Parameters.Count;
    public IFixedList<ParameterType> Parameters { get; }
    public IType Return { get; }
    IMaybeType IMaybeFunctionType.Return => Return;

    public override bool IsFullyKnown { get; }

    public override IType AccessedVia(ICapabilityConstraint capability)
        => this;

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
