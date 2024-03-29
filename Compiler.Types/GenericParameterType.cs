using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
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
    public Promise<IDeclaredUserType> DeclaringTypePromise { get; }
    public IDeclaredUserType DeclaringType => DeclaringTypePromise.Result;

    public GenericParameter Parameter { get; }

    public StandardName Name => Parameter.Name;

    public override bool IsFullyKnown => true;

    public GenericParameterType(Promise<IDeclaredUserType> declaringTypePromise, GenericParameter parameter)
    {
        DeclaringTypePromise = declaringTypePromise;
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
            && (ReferenceEquals(DeclaringTypePromise, otherType.DeclaringTypePromise)
                 || DeclaringType == otherType.DeclaringType)
            && Parameter == otherType.Parameter;
    }

    public override int GetHashCode()
    {
        if (!DeclaringTypePromise.IsFulfilled)
            return HashCode.Combine(DeclaringTypePromise, Parameter);
        return HashCode.Combine(DeclaringType, Parameter);
    }
    #endregion

    public override string ToSourceCodeString() => $"{DeclaringTypePromise}.{Parameter.Name}";

    public override string ToILString() => $"{DeclaringTypePromise}.{Parameter.Name}";
}
