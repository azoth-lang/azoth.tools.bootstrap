using System;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The type introduced by a generic parameter. This is not the concrete type is is substituted with.
/// Rather this is the type variable (e.g. <c>T</c>) that can be used a type name in type expressions.
/// </summary>
public sealed class GenericParameterType : DataType
{
    public GenericParameterType(BareObjectType declaringType, GenericParameter parameter)
    {
        if (!declaringType.GenericParameters.Contains(parameter))
            throw new ArgumentException("Must be declared with the type", nameof(parameter));
        DeclaringType = declaringType;
        Parameter = parameter;
    }

    public BareObjectType DeclaringType { get; }

    public GenericParameter Parameter { get; }
    public override bool IsKnown => true;

    // TODO the type semantics isn't actually known because it is generic
    public override TypeSemantics Semantics => TypeSemantics.Reference;

    #region Equals
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is GenericParameterType otherType
            && DeclaringType == otherType.DeclaringType
            && Parameter == otherType.Parameter;
    }

    public override int GetHashCode() => HashCode.Combine(DeclaringType, Parameter);
    #endregion

    public override string ToSourceCodeString() => $"{Parameter}";

    public override string ToILString() => $"{DeclaringType}.{Parameter}";
}
