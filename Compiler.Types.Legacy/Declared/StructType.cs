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

/// <summary>
/// A type declared with the struct keyword.
/// </summary>
/// <remarks>For now, all structs are copy structs.</remarks>
public sealed class StructType : DeclaredValueType, IDeclaredUserType
{
    public static StructType Create(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        StandardName name,
        IFixedList<GenericParameter> genericParameters,
        IFixedSet<BareReferenceType> supertypes)
    {
        Requires.That(name.GenericParameterCount == genericParameters.Count, nameof(genericParameters),
            "Count must match name count");
        return new(containingPackage, containingNamespace, isConst, name,
            genericParameters, supertypes);
    }

    private StructType(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConstType,
        StandardName name,
        IFixedList<GenericParameter> genericParameters,
        IFixedSet<BareReferenceType> supertypes)
        : base(isConstType, genericParameters)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        Name = name;
        Supertypes = supertypes;
        GenericParameterTypes = genericParameters.Select(p => new GenericParameterType(this, p)).ToFixedList();
    }

    public override IdentifierName ContainingPackage { get; }

    public override NamespaceName ContainingNamespace { get; }

    bool IDeclaredUserType.IsClass => false;
    bool IDeclaredUserType.IsAbstract => false;

    public override StandardName Name { get; }

    public override IFixedSet<BareReferenceType> Supertypes { get; }
    public override IFixedList<GenericParameterType> GenericParameterTypes { get; }

    private OrdinaryTypeConstructor? typeConstructor;

    DeclaredType IDeclaredUserType.AsDeclaredType => this;

    /// <summary>
    /// Make a version of this type for use as the default initializer parameter.
    /// </summary>
    /// <remarks>This is always `init mut` because the type is being initialized and can be mutated
    /// inside the constructor via field initializers.</remarks>
    public CapabilityType<StructType> ToDefaultInitializerSelf()
        => With(Capability.InitMutable, GenericParameterTypes);

    /// <summary>
    /// Make a version of this type for use as the return type of the default constructor.
    /// </summary>
    /// <remarks>This is always either `iso` or `const` depending on whether the type was declared
    /// with `const` because there are no parameters that could break the new object's isolation.</remarks>
    public CapabilityType<StructType> ToDefaultInitializerReturn()
        => With(IsDeclaredConst ? Capability.Constant : Capability.Isolated, GenericParameterTypes);

    /// <summary>
    /// Determine the return type of an initializer with the given parameter types.
    /// </summary>
    /// <remarks>The capability of the return type is restricted by the parameter types because the
    /// newly constructed object could contain references to them.</remarks>
    public CapabilityType<StructType> ToInitializerReturn(CapabilityType selfParameterType, IEnumerable<ParameterType> parameterTypes)
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

    public override BareValueType<StructType> With(IFixedList<IType> typeArguments)
        => BareNonVariableType.Create(this, typeArguments);

    public override CapabilityType<StructType> With(Capability capability, IFixedList<IType> typeArguments)
        => With(typeArguments).With(capability);

    public CapabilityTypeConstraint With(CapabilitySet capability, IFixedList<IType> typeArguments)
        => With(typeArguments).With(capability);

    public override OrdinaryTypeConstructor ToTypeConstructor()
        // Lazy initialize to prevent evaluation of lazy supertypes when constructing StructType
        => LazyInitializer.EnsureInitialized(ref typeConstructor, this.ConstructTypeConstructor);
    public override IPlainType? TryToPlainType() => ToTypeConstructor().TryConstructNullary();

    #region Equals
    public override bool Equals(DeclaredType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is StructType structType
            && ContainingPackage == structType.ContainingPackage
            && ContainingNamespace == structType.ContainingNamespace
            && IsDeclaredConst == structType.IsDeclaredConst
            && Name == structType.Name
            && GenericParameters.Equals(structType.GenericParameters);
        // Supertypes and GenericParameterTypes are not considered because they are derived. Also,
        // that prevents infinite recursion.
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
        if (GenericParameters.IsEmpty) return;

        builder.Append('[');
        builder.AppendJoin(", ", GenericParameters);
        builder.Append(']');
    }
}
