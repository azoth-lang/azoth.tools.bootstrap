using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The type introduced by a generic parameter. This is not the concrete type it is substituted with.
/// Rather this is the type variable (e.g. <c>T</c>) that can be used as a type name in type expressions.
/// </summary>
public sealed class GenericParameterType : NonEmptyType, INonVoidType
{
    public IDeclaredUserType DeclaringType { get; }

    public GenericParameter Parameter { get; }

    public IdentifierName Name => Parameter.Name;

    internal GenericParameterType(IDeclaredUserType declaringType, GenericParameter parameter)
    {
        DeclaringType = declaringType;
        Parameter = parameter;
    }

    IMaybeNonVoidType IMaybeNonVoidType.WithoutWrite() => this;

    IMaybeType IMaybeType.AccessedVia(IMaybePseudotype contextType) => (IMaybeType)AccessedVia(contextType);

    public override IType AccessedVia(ICapabilityConstraint capability)
    {
        // Independent type parameters are not affected by the capability
        if (Parameter.HasIndependence) return this;
        return capability switch
        {
            Capability c => CapabilityViewpointType.Create(c, this),
            CapabilitySet c => SelfViewpointType.Create(c, this),
            _ => throw ExhaustiveMatch.Failed(capability),
        };
    }
    IMaybeType IMaybeType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);

    #region Equals
    public override bool Equals(IMaybeExpressionType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is GenericParameterType otherType
            && DeclaringType.Equals(otherType.DeclaringType) // Must use Equals because they are interface types
            && Parameter.Equals(otherType.Parameter);
    }

    public override int GetHashCode()
        => HashCode.Combine(DeclaringType, Parameter);
    #endregion

    public override GenericParameterAntetype ToAntetype()
    {
        var declaringAntetype = DeclaringType.ToAntetype();
        return new GenericParameterAntetype((UserDeclaredGenericAntetype)declaringAntetype, declaringAntetype.GenericParameters.Single(p => p.Name == Name));
    }
    INonVoidAntetype INonVoidType.ToAntetype() => ToAntetype();

    public override string ToSourceCodeString() => $"{DeclaringType}.{Parameter.Name}";

    public override string ToILString() => $"{DeclaringType}.{Parameter.Name}";
}
