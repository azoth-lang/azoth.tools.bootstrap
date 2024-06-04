using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using Compiler.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A type for a function, closure, or method.
/// </summary>
public sealed class FunctionType : NonEmptyType
{
    public FunctionType(IEnumerable<Parameter> parameters, Return @return)
    {
        Parameters = parameters.ToFixedList();
        Return = @return;
        IsFullyKnown = Parameters.All(p => p.IsFullyKnown) && @return.IsFullyKnown;
    }

    public int Arity => Parameters.Count;
    public IFixedList<Parameter> Parameters { get; }
    public Return Return { get; }

    public override bool IsFullyKnown { get; }

    #region Equality
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionType otherType
               && Parameters.ItemsEqual(otherType.Parameters)
               && Return == otherType.Return;
    }

    public override int GetHashCode() => HashCode.Combine(Parameters, Return);
    #endregion

    public override IMaybeExpressionAntetype ToAntetype()
    {
        var parameters = Parameters.Select(p => p.Type.ToAntetype()).OfType<INonVoidAntetype>().ToFixedList();
        if (parameters.Count != Parameters.Count)
            // Not all parameters are known and non-void
            return UnknownAntetype.Instance;

        if (Return.Type.ToAntetype() is not IAntetype returnAntetype)
            return UnknownAntetype.Instance;

        return new FunctionAntetype(parameters, returnAntetype);
    }

    public override string ToSourceCodeString()
        => $"({string.Join(", ", Parameters.Select(t => t.ToSourceCodeString()))}) -> {Return.ToSourceCodeString()}";

    public override string ToILString()
        => $"({string.Join(", ", Parameters.Select(t => t.ToILString()))}) -> {Return.ToILString()}";
}
