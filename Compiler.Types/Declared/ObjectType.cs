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

public sealed class ObjectType : DeclaredReferenceType
{
    private static readonly FixedSet<BareReferenceType> AnyType
        = Declared.AnyType.Instance.BareType.Yield().ToFixedSet<BareReferenceType>();
    private static readonly Promise<FixedSet<BareReferenceType>> AnyTypePromise = new(AnyType);

    public static ObjectType CreateClass(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name)
        => new(containingPackage, containingNamespace, isAbstract, isConst, isClass: true, name,
            FixedList.Empty<GenericParameterType>(), AnyTypePromise);

    public static ObjectType CreateTrait(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name)
        => new(containingPackage, containingNamespace, isAbstract: true, isConst, isClass: false, name,
            FixedList.Empty<GenericParameterType>(), AnyTypePromise);

    public static ObjectType CreateClass(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name,
        IFixedList<GenericParameterType> genericParameterTypes,
        IPromise<FixedSet<BareReferenceType>> superTypes)
        => new(containingPackage, containingNamespace, isAbstract, isConst, isClass: true,
            StandardTypeName.Create(name, genericParameterTypes.Count), genericParameterTypes, superTypes);

    public static ObjectType CreateClass(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        StandardTypeName name,
        IFixedList<GenericParameterType> genericParametersTypes,
        IPromise<FixedSet<BareReferenceType>> superTypes)
    {
        Requires.That(nameof(genericParametersTypes), name.GenericParameterCount == genericParametersTypes.Count, "Count must match name count");
        return new(containingPackage, containingNamespace, isAbstract, isConst, isClass: true, name,
            genericParametersTypes, superTypes);
    }

    public static ObjectType CreateTrait(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        StandardTypeName name,
        IFixedList<GenericParameterType> genericParametersTypes,
        IPromise<FixedSet<BareReferenceType>> superTypes)
    {
        Requires.That(nameof(genericParametersTypes), name.GenericParameterCount == genericParametersTypes.Count,
            "Count must match name count");
        return new(containingPackage, containingNamespace, isAbstract: true, isConst, isClass: false, name,
            genericParametersTypes, superTypes);
    }

    public static ObjectType CreateClass(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name,
        params GenericParameter[] genericParameters)
    {
        var declaringTypePromise = new Promise<ObjectType>();
        var genericParametersTypes = genericParameters
            .Select(p => new GenericParameterType(declaringTypePromise, p)).ToFixedList();
        return new(containingPackage, containingNamespace, isAbstract, isConst, isClass: true,
            StandardTypeName.Create(name, genericParameters.Length), genericParametersTypes, AnyTypePromise);
    }

    public static ObjectType CreateTrait(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name,
        params GenericParameter[] genericParameters)
    {
        var declaringTypePromise = new Promise<ObjectType>();
        var genericParametersTypes = genericParameters
            .Select(p => new GenericParameterType(declaringTypePromise, p)).ToFixedList();
        return new(containingPackage, containingNamespace, isAbstract: true, isConst, isClass: false,
            StandardTypeName.Create(name, genericParameters.Length), genericParametersTypes, AnyTypePromise);
    }

    private ObjectType(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConstType,
        bool isClass,
        StandardTypeName name,
        IFixedList<GenericParameterType> genericParametersTypes,
        IPromise<FixedSet<BareReferenceType>> supertypes)
        : base(isConstType, isAbstract, genericParametersTypes)
    {
        //if (!supertypes.Contains(BareReferenceType.Any))
        //    throw new ArgumentException("All object types must be subtypes of `Any`", nameof(supertypes));
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        IsClass = isClass;
        Name = name;
        this.supertypes = supertypes;
        // Fulfill the declaring type promise so the parameters are associated to this type
        var declaringTypePromise = genericParametersTypes.Select(t => t.DeclaringTypePromise)
                                                         .Distinct().SingleOrDefault();
        declaringTypePromise?.Fulfill(this);
    }

    public override SimpleName ContainingPackage { get; }

    public override NamespaceName ContainingNamespace { get; }

    public override StandardTypeName Name { get; }

    /// <summary>
    /// Whether this type is a `class` (as opposed to a `trait`)
    /// </summary>
    public bool IsClass { get; }

    private readonly IPromise<FixedSet<BareReferenceType>> supertypes;
    public override FixedSet<BareReferenceType> Supertypes => supertypes.Result;

    /// <summary>
    /// Make a version of this type for use as the default constructor parameter.
    /// </summary>
    /// <remarks>This is always `init mut` because the type is being initialized and can be mutated
    /// inside the constructor via field initializers.</remarks>
    public ReferenceType<ObjectType> ToDefaultConstructorSelf()
        => With(Capability.InitMutable, GenericParameterTypes);

    /// <summary>
    /// Make a version of this type for use as the return type of the default constructor.
    /// </summary>
    /// <remarks>This is always either `iso` or `const` depending on whether the type was declared
    /// with `const` because there are no parameters that could break the new objects isolation.</remarks>
    public ReferenceType<ObjectType> ToDefaultConstructorReturn()
        => With(IsDeclaredConst ? Capability.Constant : Capability.Isolated, GenericParameterTypes);

    /// <summary>
    /// Determine the return type of a constructor with the given parameter types.
    /// </summary>
    /// <remarks>The capability of the return type is restricted by the parameter types because the
    /// newly constructed object could contain references to them.</remarks>
    public ReferenceType<ObjectType> ToConstructorReturn(ReferenceType selfParameterType, IEnumerable<Parameter> parameterTypes)
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

    public override BareReferenceType<ObjectType> With(IFixedList<DataType> typeArguments)
        => BareType.Create(this, typeArguments);

    public override ReferenceType<ObjectType> With(Capability capability, IFixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    public ObjectTypeConstraint With(CapabilitySet capability, IFixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    /// <summary>
    /// Make a version of this type that is the default read reference capability for the type. That
    /// is either read-only or constant.
    /// </summary>
    public override ReferenceType<ObjectType> WithRead(IFixedList<DataType> typeArguments)
        => With(IsDeclaredConst ? Capability.Constant : Capability.Read, typeArguments);

    /// <summary>
    /// Make a version of this type that is the default mutate reference capability for the type.
    /// For constant types, that isn't allowed and a constant reference is returned.
    /// </summary>
    public ReferenceType<ObjectType> WithMutate(IFixedList<DataType> typeArguments)
        => With(IsDeclaredConst ? Capability.Constant : Capability.Mutable, typeArguments);

    #region Equals
    public override bool Equals(DeclaredType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ObjectType objectType
            && ContainingPackage == objectType.ContainingPackage
            && ContainingNamespace == objectType.ContainingNamespace
            && IsAbstract == objectType.IsAbstract
            && IsDeclaredConst == objectType.IsDeclaredConst
            && Name == objectType.Name
            && GenericParameters.ItemsEquals(objectType.GenericParameters);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingPackage, ContainingNamespace, IsAbstract, IsDeclaredConst, Name, GenericParameters);
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
