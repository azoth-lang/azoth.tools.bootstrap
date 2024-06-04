using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

public sealed class ObjectType : DeclaredReferenceType, IDeclaredUserType
{
    private static readonly IFixedSet<BareReferenceType> AnyTypeSet
        = Declared.AnyType.Instance.BareType.Yield().ToFixedSet<BareReferenceType>();
    private static readonly Lazy<IFixedSet<BareReferenceType>> LazyAnyTypeSet = new(AnyTypeSet);

    public static ObjectType CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name)
        => new(containingPackage, containingNamespace, isAbstract, isConst, isClass: true, name,
            FixedList.Empty<GenericParameter>(), LazyAnyTypeSet);

    public static ObjectType CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name)
        => new(containingPackage, containingNamespace, isAbstract: true, isConst, isClass: false, name,
            FixedList.Empty<GenericParameter>(), LazyAnyTypeSet);

    public static ObjectType CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name,
        IFixedList<GenericParameter> genericParameters,
        Lazy<IFixedSet<BareReferenceType>> supertypes)
        => new(containingPackage, containingNamespace, isAbstract, isConst, isClass: true,
            StandardName.Create(name, genericParameters.Count), genericParameters, supertypes);

    public static ObjectType CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        StandardName name,
        IFixedList<GenericParameter> genericParameters,
        Lazy<IFixedSet<BareReferenceType>> supertypes)
    {
        Requires.That(nameof(genericParameters), name.GenericParameterCount == genericParameters.Count, "Count must match name count");
        return new(containingPackage, containingNamespace, isAbstract, isConst, isClass: true, name,
            genericParameters, supertypes);
    }

    public static ObjectType CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        StandardName name,
        IFixedList<GenericParameter> genericParameters,
        Lazy<IFixedSet<BareReferenceType>> supertypes)
    {
        Requires.That(nameof(genericParameters), name.GenericParameterCount == genericParameters.Count,
            "Count must match name count");
        return new(containingPackage, containingNamespace, isAbstract: true, isConst, isClass: false, name,
            genericParameters, supertypes);
    }

    public static ObjectType CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name,
        params GenericParameter[] genericParameters)
    {
        return new(containingPackage, containingNamespace, isAbstract, isConst, isClass: true,
            StandardName.Create(name, genericParameters.Length), genericParameters.ToFixedList(), LazyAnyTypeSet);
    }

    public static ObjectType CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name,
        params GenericParameter[] genericParameters)
    {
        return new(containingPackage, containingNamespace, isAbstract: true, isConst, isClass: false,
            StandardName.Create(name, genericParameters.Length), genericParameters.ToFixedList(), LazyAnyTypeSet);
    }

    private ObjectType(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConstType,
        bool isClass,
        StandardName name,
        IFixedList<GenericParameter> genericParameters,
        Lazy<IFixedSet<BareReferenceType>> supertypes)
        : base(isConstType, isAbstract, isClass, genericParameters)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        Name = name;
        this.supertypes = supertypes;
        GenericParameterTypes = GenericParameters.Select(p => new GenericParameterType(this, p)).ToFixedList();
    }

    public override IdentifierName ContainingPackage { get; }

    public override NamespaceName ContainingNamespace { get; }

    public override StandardName Name { get; }

    private readonly Lazy<IFixedSet<BareReferenceType>> supertypes;
    public override IFixedSet<BareReferenceType> Supertypes => supertypes.Value;
    public override IFixedList<GenericParameterType> GenericParameterTypes { get; }

    DeclaredType IDeclaredUserType.AsDeclaredType() => this;

    /// <summary>
    /// Make a version of this type for use as the default constructor parameter.
    /// </summary>
    /// <remarks>This is always `init mut` because the type is being initialized and can be mutated
    /// inside the constructor via field initializers.</remarks>
    public CapabilityType<ObjectType> ToDefaultConstructorSelf()
        => With(Capability.InitMutable, GenericParameterTypes);

    /// <summary>
    /// Make a version of this type for use as the return type of the default constructor.
    /// </summary>
    /// <remarks>This is always either `iso` or `const` depending on whether the type was declared
    /// with `const` because there are no parameters that could break the new objects isolation.</remarks>
    public CapabilityType<ObjectType> ToDefaultConstructorReturn()
        => With(IsDeclaredConst ? Capability.Constant : Capability.Isolated, GenericParameterTypes);

    /// <summary>
    /// Determine the return type of a constructor with the given parameter types.
    /// </summary>
    /// <remarks>The capability of the return type is restricted by the parameter types because the
    /// newly constructed object could contain references to them.</remarks>
    public CapabilityType<ObjectType> ToConstructorReturn(CapabilityType selfParameterType, IEnumerable<Parameter> parameterTypes)
    {
        if (IsDeclaredConst) return With(Capability.Constant, GenericParameterTypes);
        // Read only self constructors cannot return `mut` or `iso`
        if (!selfParameterType.AllowsWrite)
            return With(Capability.Read, GenericParameterTypes);
        foreach (var parameterType in parameterTypes)
            switch (parameterType.Type)
            {
                case CapabilityType when parameterType.IsLent:
                case CapabilityType { IsConstantReference: true }:
                case CapabilityType { IsIsolatedReference: true }:
                case OptionalType { Referent: CapabilityType } when parameterType.IsLent:
                case OptionalType { Referent: CapabilityType { IsConstantReference: true } }:
                case OptionalType { Referent: CapabilityType { IsIsolatedReference: true } }:
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

    public override CapabilityType<ObjectType> With(Capability capability, IFixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    public CapabilityTypeConstraint With(CapabilitySet capability, IFixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    /// <summary>
    /// Make a version of this type that is the default mutate reference capability for the type.
    /// For constant types, that isn't allowed and a constant reference is returned.
    /// </summary>
    public CapabilityType<ObjectType> WithMutate(IFixedList<DataType> typeArguments)
        => With(IsDeclaredConst ? Capability.Constant : Capability.Mutable, typeArguments);

    public override IDeclaredAntetype ToAntetype()
        => IsGeneric ? new UserDeclaredGenericAntetype() : new UserNonGenericNominalAntetype();

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
            && GenericParameters.ItemsEqual(objectType.GenericParameters);
        // Supertypes and GenericParameterTypes are not considered because they are derived. Also,
        // that prevents infinite recursion.
    }

    public bool Equals(IDeclaredUserType? other) => Equals(other as DeclaredType);

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
