using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A type for a function, closure, or method.
/// </summary>
public sealed class FunctionType : NonEmptyType
{
    public FunctionType(IFixedList<Parameter> parameters, Return @return)
    {
        Parameters = parameters;
        Return = @return;
        IsFullyKnown = parameters.All(p => p.IsFullyKnown) && @return.IsFullyKnown;
        Semantics = TypeSemantics.Reference;
    }

    public IFixedList<Parameter> Parameters { get; }
    public Return Return { get; }

    public override bool IsFullyKnown { get; }
    public override TypeSemantics Semantics { get; }

    #region Equality
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionType otherType
               && Parameters.ItemsEquals(otherType.Parameters)
               && Return == otherType.Return;
    }

    public override int GetHashCode() => HashCode.Combine(Parameters, Return);
    #endregion

    public override string ToSourceCodeString()
        => $"({string.Join(", ", Parameters.Select(t => t.ToSourceCodeString()))}) -> {Return.ToSourceCodeString()}";

    public override string ToILString()
        => $"({string.Join(", ", Parameters.Select(t => t.ToILString()))}) -> {Return.ToILString()}";
}
