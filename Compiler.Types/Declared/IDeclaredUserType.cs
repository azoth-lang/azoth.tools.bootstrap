using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

/// <summary>
/// A declared type that is not a simple type or the `Any` type. Thus it is declared by the "user".
/// </summary>
/// <remarks>Some intrinsic types also fall into this category.</remarks>
[Closed(typeof(ObjectType), typeof(StructType))]
public interface IDeclaredUserType : IEquatable<IDeclaredUserType>
{
    IdentifierName ContainingPackage { get; }
    NamespaceName ContainingNamespace { get; }
    bool IsDeclaredConst { get; }
    bool IsClass { get; }
    StandardTypeName Name { get; }
    IFixedList<GenericParameter> GenericParameters { get; }
    bool HasIndependentGenericParameters { get; }
    bool AllowsVariance { get; }
    IFixedList<GenericParameterType> GenericParameterTypes { get; }
    bool IsGeneric { get; }
    FixedSet<BareReferenceType> Supertypes { get; }

    BareType With(IFixedList<DataType> typeArguments);
    CapabilityType With(Capability capability, IFixedList<DataType> typeArguments);
    CapabilityTypeConstraint With(CapabilitySet capability, IFixedList<DataType> typeArguments);
    CapabilityType WithRead(IFixedList<DataType> typeArguments);
}
