using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

/// <summary>
/// The type introduced by a generic parameter. This is not the concrete type it is substituted with.
/// Rather this is the type variable (e.g. <c>T</c>) that can be used as a type name in type expressions.
/// </summary>
public sealed class GenericParameterType : NonEmptyType, INonVoidType
{
    public OrdinaryTypeConstructor TypeConstructor { get; }

    public TypeConstructor.Parameter Parameter { get; }

    public IdentifierName Name => Parameter.Name;

    internal GenericParameterType(OrdinaryTypeConstructor typeConstructor, TypeConstructor.Parameter parameter)
    {
        TypeConstructor = typeConstructor;
        Parameter = parameter;
    }

    IMaybeNonVoidType IMaybeNonVoidType.WithoutWrite() => this;

    IMaybeType IMaybeType.AccessedVia(IMaybePseudotype contextType) => AccessedVia(contextType);

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

    public override Decorated.GenericParameterType ToDecoratedType() => new(ToPlainType());

    IMaybeType IMaybeType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);

    #region Equals
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is GenericParameterType otherType
            && TypeConstructor.Equals(otherType.TypeConstructor) // Must use Equals because they are interface types
            && Parameter.Equals(otherType.Parameter);
    }

    public override int GetHashCode()
        => HashCode.Combine(TypeConstructor, Parameter);
    #endregion

    public override GenericParameterPlainType ToPlainType()
        => new(TypeConstructor, TypeConstructor.Parameters.Single(p => p.Name == Name));
    INonVoidPlainType INonVoidType.ToPlainType() => ToPlainType();

    public override string ToSourceCodeString() => $"{TypeConstructor}.{Parameter.Name}";

    public override string ToILString() => $"{TypeConstructor}.{Parameter.Name}";
}
