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
    public FunctionType(FixedList<ParameterType> parameterTypes, ReturnType returnType)
    {
        ParameterTypes = parameterTypes;
        ReturnType = returnType;
        IsFullyKnown = parameterTypes.All(p => p.IsFullyKnown) && returnType.IsFullyKnown;
        Semantics = TypeSemantics.Reference;
    }

    public FixedList<ParameterType> ParameterTypes { get; }
    public ReturnType ReturnType { get; }

    public override bool IsFullyKnown { get; }
    public override TypeSemantics Semantics { get; }

    #region Equality
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionType otherType
               && ParameterTypes.Equals(otherType.ParameterTypes)
               && ReturnType == otherType.ReturnType;
    }

    public override int GetHashCode() => HashCode.Combine(ParameterTypes, ReturnType);
    #endregion

    public override string ToSourceCodeString()
        => $"({string.Join(", ", ParameterTypes.Select(t => t.ToSourceCodeString()))}) -> {ReturnType.ToSourceCodeString()}";

    public override string ToILString()
        => $"({string.Join(", ", ParameterTypes.Select(t => t.ToILString()))}) -> {ReturnType.ToILString()}";
}
