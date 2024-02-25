using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The type introduced by a generic parameter. This is not the concrete type it is substituted with.
/// Rather this is the type variable (e.g. <c>T</c>) that can be used a type name in type expressions.
/// </summary>
public sealed class GenericParameterType : NonEmptyType
{
    public Promise<ObjectType> DeclaringTypePromise { get; }
    public ObjectType DeclaringType => DeclaringTypePromise.Result;

    public GenericParameter Parameter { get; }

    public StandardTypeName Name => Parameter.Name;

    public override bool IsFullyKnown => true;

    // TODO the type semantics isn't actually known because it is generic
    public override TypeSemantics Semantics => TypeSemantics.CopyValue;

    public GenericParameterType(Promise<ObjectType> declaringTypePromise, GenericParameter parameter)
    {
        DeclaringTypePromise = declaringTypePromise;
        Parameter = parameter;
    }

    public override DataType AccessedVia(ICapabilityConstraint capability)
    {
        // Independent type parameters are not affected by the capability
        if (Parameter.ParameterVariance == ParameterVariance.Independent) return this;
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
