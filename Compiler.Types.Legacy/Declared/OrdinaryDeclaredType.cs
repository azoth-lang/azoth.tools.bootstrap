using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

public sealed class OrdinaryDeclaredType : DeclaredType, IDeclaredUserType
{
    private static readonly IFixedSet<BareNonVariableType> AnyTypeSet
     = AnyType.Instance.BareType.Yield().ToFixedSet();

    public static OrdinaryDeclaredType CreateStruct(
    IdentifierName containingPackage,
    NamespaceName containingNamespace,
    bool isConst,
    StandardName name,
    IFixedList<GenericParameter> genericParameters,
    IFixedSet<BareNonVariableType> supertypes)
    {
        Requires.That(name.GenericParameterCount == genericParameters.Count, nameof(genericParameters),
            "Count must match name count");
        return new(containingPackage, containingNamespace, isAbstract: false, isConst, TypeKind.Struct, name,
            genericParameters, supertypes);
    }

    public static OrdinaryDeclaredType CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name)
        => new(containingPackage, containingNamespace, isAbstract, isConst, TypeKind.Class, name,
            [], AnyTypeSet);

    public static OrdinaryDeclaredType CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name)
        => new(containingPackage, containingNamespace, isAbstract: true, isConst, TypeKind.Trait,
            name, [], AnyTypeSet);

    public static OrdinaryDeclaredType CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name,
        IFixedList<GenericParameter> genericParameters,
        IFixedSet<BareNonVariableType> supertypes)
        => new(containingPackage, containingNamespace, isAbstract, isConst, TypeKind.Class,
            StandardName.Create(name, genericParameters.Count), genericParameters, supertypes);

    public static OrdinaryDeclaredType CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        StandardName name,
        IFixedList<GenericParameter> genericParameters,
        IFixedSet<BareNonVariableType> supertypes)
    {
        Requires.That(name.GenericParameterCount == genericParameters.Count, nameof(genericParameters), "Count must match name count");
        return new(containingPackage, containingNamespace, isAbstract, isConst, TypeKind.Class, name,
            genericParameters, supertypes);
    }

    public static OrdinaryDeclaredType CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        StandardName name,
        IFixedList<GenericParameter> genericParameters,
        IFixedSet<BareNonVariableType> supertypes)
    {
        Requires.That(name.GenericParameterCount == genericParameters.Count, nameof(genericParameters),
            "Count must match name count");
        return new(containingPackage, containingNamespace, isAbstract: true, isConst, TypeKind.Trait, name,
            genericParameters, supertypes);
    }

    public static OrdinaryDeclaredType CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name,
        params GenericParameter[] genericParameters)
    {
        return new(containingPackage, containingNamespace, isAbstract, isConst, TypeKind.Class,
            StandardName.Create(name, genericParameters.Length), genericParameters.ToFixedList(), AnyTypeSet);
    }

    public static OrdinaryDeclaredType CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name,
        params GenericParameter[] genericParameters)
    {
        return new(containingPackage, containingNamespace, isAbstract: true, isConst, TypeKind.Trait,
            StandardName.Create(name, genericParameters.Length), genericParameters.ToFixedList(), AnyTypeSet);
    }

    private OrdinaryDeclaredType(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isDeclaredConst,
        TypeKind kind,
        StandardName name,
        IFixedList<GenericParameter> genericParameters,
        IFixedSet<BareNonVariableType> supertypes)
        : base(isDeclaredConst, genericParameters)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        IsAbstract = isAbstract;
        Kind = kind;
        Name = name;
        Supertypes = supertypes;
        GenericParameterTypes = GenericParameters.Select(p => new GenericParameterType(this, p)).ToFixedList();
    }

    public override IdentifierName ContainingPackage { get; }

    public override NamespaceName ContainingNamespace { get; }

    public override TypeSemantics Semantics
        => Kind == TypeKind.Struct ? TypeSemantics.Value : TypeSemantics.Reference;

    public TypeKind Kind { get; }

    public override bool CanHaveFields => Kind != TypeKind.Trait;

    public override StandardName Name { get; }

    public override IFixedSet<BareNonVariableType> Supertypes { get; }
    public override IFixedList<GenericParameterType> GenericParameterTypes { get; }

    private OrdinaryTypeConstructor? typeConstructor;

    DeclaredType IDeclaredUserType.AsDeclaredType => this;
    public override bool CanBeSupertype => Kind != TypeKind.Struct;
    public bool IsAbstract { get; }

    /// <summary>
    /// Make a version of this type for use as the default constructor or initializer parameter.
    /// </summary>
    /// <remarks>This is always `init mut` because the type is being initialized and can be mutated
    /// inside the constructor via field initializers.</remarks>
    public CapabilityType ToDefaultConstructorSelf()
        => With(Capability.InitMutable, GenericParameterTypes);

    /// <summary>
    /// Make a version of this type for use as the return type of the default constructor or initializer.
    /// </summary>
    /// <remarks>This is always either `iso` or `const` depending on whether the type was declared
    /// with `const` because there are no parameters that could break the new objects isolation.</remarks>
    public CapabilityType ToDefaultConstructorReturn()
        => With(IsDeclaredConst ? Capability.Constant : Capability.Isolated, GenericParameterTypes);

    /// <summary>
    /// Determine the return type of a constructor or initializer with the given parameter types.
    /// </summary>
    /// <remarks>The capability of the return type is restricted by the parameter types because the
    /// newly constructed object could contain references to them.</remarks>
    public CapabilityType ToConstructorReturn(CapabilityType selfParameterType, IEnumerable<ParameterType> parameterTypes)
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
                    continue;
                default:
                    return With(Capability.Mutable, GenericParameterTypes);
            }

        return With(Capability.Isolated, GenericParameterTypes);
    }

    public override BareNonVariableType With(IFixedList<IType> typeArguments)
        => BareNonVariableType.Create(this, typeArguments);

    public override CapabilityType With(Capability capability, IFixedList<IType> typeArguments)
        => With(typeArguments).With(capability);

    public CapabilityTypeConstraint With(CapabilitySet capability, IFixedList<IType> typeArguments)
        => With(typeArguments).With(capability);

    /// <summary>
    /// Make a version of this type that is the default mutate reference capability for the type.
    /// For constant types, that isn't allowed and a constant reference is returned.
    /// </summary>
    public CapabilityType WithMutate(IFixedList<IType> typeArguments)
        => With(IsDeclaredConst ? Capability.Constant : Capability.Mutable, typeArguments);

    public override OrdinaryTypeConstructor ToTypeConstructor()
        // Lazy initialize to prevent evaluation of lazy supertypes when constructing
        => LazyInitializer.EnsureInitialized(ref typeConstructor, this.ConstructTypeConstructor);
    public override IPlainType? TryToPlainType() => ToTypeConstructor().TryConstructNullary();

    #region Equals
    public override bool Equals(DeclaredType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OrdinaryDeclaredType otherType
            && ContainingPackage == otherType.ContainingPackage
            && ContainingNamespace == otherType.ContainingNamespace
            && IsAbstract == otherType.IsAbstract
            && IsDeclaredConst == otherType.IsDeclaredConst
            && Kind == otherType.Kind
            && Name == otherType.Name
            && GenericParameters.Equals(otherType.GenericParameters);
        // Supertypes and GenericParameterTypes are not considered because they are derived. Also,
        // that prevents infinite recursion.
    }

    public bool Equals(IDeclaredUserType? other) => Equals(other as DeclaredType);

    public override int GetHashCode()
        => HashCode.Combine(ContainingPackage, ContainingNamespace, IsAbstract, IsDeclaredConst, Kind, Name, GenericParameters);
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
        if (GenericParameters.IsEmpty) return;

        builder.Append('[');
        builder.AppendJoin(", ", GenericParameters);
        builder.Append(']');
    }
}
