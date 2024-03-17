using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

/// <summary>
/// A type declared with the struct keyword.
/// </summary>
/// <remarks>For now, all structs are copy structs.</remarks>
public sealed class StructType : DeclaredValueType, IDeclaredUserType
{
    public static StructType Create(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        StandardTypeName name,
        IFixedList<GenericParameterType> genericParametersTypes,
        IPromise<FixedSet<BareReferenceType>> superTypes)
    {
        Requires.That(nameof(genericParametersTypes), name.GenericParameterCount == genericParametersTypes.Count,
            "Count must match name count");
        return new(containingPackage, containingNamespace, isConst, name,
            genericParametersTypes, superTypes);
    }

    private StructType(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isConstType,
        StandardTypeName name,
        IFixedList<GenericParameterType> genericParametersTypes,
        IPromise<FixedSet<BareReferenceType>> supertypes)
        : base(isConstType, genericParametersTypes)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        Name = name;
        this.supertypes = supertypes;
        // Fulfill the declaring type promise so the parameters are associated to this type
        var declaringTypePromise = genericParametersTypes.Select(t => t.DeclaringTypePromise)
                                                         .Distinct().SingleOrDefault();
        declaringTypePromise?.Fulfill(this);
    }

    public override SimpleName ContainingPackage { get; }

    public override NamespaceName ContainingNamespace { get; }

    bool IDeclaredUserType.IsClass => false;

    public override StandardTypeName Name { get; }

    private readonly IPromise<FixedSet<BareReferenceType>> supertypes;
    public override FixedSet<BareReferenceType> Supertypes => supertypes.Result;

    /// <summary>
    /// Make a version of this type for use as the default initializer parameter.
    /// </summary>
    /// <remarks>This is always `init mut` because the type is being initialized and can be mutated
    /// inside the constructor via field initializers.</remarks>
    public ValueType<StructType> ToDefaultInitializerSelf()
        => With(Capability.InitMutable, GenericParameterTypes);

    /// <summary>
    /// Make a version of this type for use as the return type of the default constructor.
    /// </summary>
    /// <remarks>This is always either `iso` or `const` depending on whether the type was declared
    /// with `const` because there are no parameters that could break the new objects isolation.</remarks>
    public ValueType<StructType> ToDefaultInitializerReturn()
        => With(IsDeclaredConst ? Capability.Constant : Capability.Isolated, GenericParameterTypes);

    /// <summary>
    /// Determine the return type of a constructor with the given parameter types.
    /// </summary>
    /// <remarks>The capability of the return type is restricted by the parameter types because the
    /// newly constructed object could contain references to them.</remarks>
    public ValueType<StructType> ToInitializerReturn(ValueType selfParameterType, IEnumerable<Parameter> parameterTypes)
    {
        if (IsDeclaredConst) return With(Capability.Constant, GenericParameterTypes);
        // Read only self constructors cannot return `mut` or `iso`
        if (!selfParameterType.AllowsWrite)
            return With(Capability.Read, GenericParameterTypes);
        foreach (var parameterType in parameterTypes)
            switch (parameterType.Type)
            {
                case ReferenceType when parameterType.IsLent:
                case ReferenceType { IsConstantReference: true }:
                case ReferenceType { IsIsolatedReference: true }:
                case OptionalType { Referent: ReferenceType } when parameterType.IsLent:
                case OptionalType { Referent: ReferenceType { IsConstantReference: true } }:
                case OptionalType { Referent: ReferenceType { IsIsolatedReference: true } }:
                case ValueType:
                case EmptyType:
                case UnknownType:
                    continue;
                default:
                    return With(Capability.Mutable, GenericParameterTypes);
            }

        return With(Capability.Isolated, GenericParameterTypes);
    }


    public override BareValueType<StructType> With(IFixedList<DataType> typeArguments)
        => BareType.Create(this, typeArguments);

    public override ValueType<StructType> With(Capability capability, IFixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    public CapabilityTypeConstraint With(CapabilitySet capability, IFixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    #region Equals
    public override bool Equals(DeclaredType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ObjectType objectType
            && ContainingPackage == objectType.ContainingPackage
            && ContainingNamespace == objectType.ContainingNamespace
            && IsDeclaredConst == objectType.IsDeclaredConst
            && Name == objectType.Name
            && GenericParameters.ItemsEquals(objectType.GenericParameters);
    }

    public bool Equals(IDeclaredUserType? other) => Equals(other as DeclaredType);

    public override int GetHashCode()
        => HashCode.Combine(ContainingPackage, ContainingNamespace, IsDeclaredConst, Name, GenericParameters);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder);
        return builder.ToString();
    }

    public void ToString(StringBuilder builder)
    {
        builder.Append(ContainingPackage);
        builder.Append("::.");
        builder.Append(ContainingNamespace);
        if (ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(Name.ToBareString());
        if (!GenericParameters.Any()) return;

        builder.Append('[');
        builder.AppendJoin(", ", GenericParameters);
        builder.Append(']');
    }
}
