using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The type introduced by a generic parameter. This is not the concrete type it is substituted with.
/// Rather this is the type variable (e.g. <c>T</c>) that can be used as a type name in type expressions.
/// </summary>
public sealed class GenericParameterType : NonEmptyType
{
    public IDeclaredUserType DeclaringType { get; }

    public GenericParameter Parameter { get; }

    public StandardName Name => Parameter.Name;

    public override bool IsFullyKnown => true;

    internal GenericParameterType(IDeclaredUserType declaringType, GenericParameter parameter)
    {
        DeclaringType = declaringType;
        Parameter = parameter;
    }

    public override DataType AccessedVia(ICapabilityConstraint capability)
    {
        // Independent type parameters are not affected by the capability
        if (Parameter.HasIndependence) return this;
        return capability switch
        {
            Capability c => CapabilityViewpointType.Create(c, this),
            CapabilitySet c => new SelfViewpointType(c, this),
            _ => throw ExhaustiveMatch.Failed(capability),
        };
    }

    #region Equals
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is GenericParameterType otherType
            && DeclaringType == otherType.DeclaringType
            && Parameter == otherType.Parameter;
    }

    public override int GetHashCode()
        => HashCode.Combine(DeclaringType, Parameter);
    #endregion

    public override string ToSourceCodeString() => $"{DeclaringType}.{Parameter.Name}";

    public override string ToILString() => $"{DeclaringType}.{Parameter.Name}";
}
