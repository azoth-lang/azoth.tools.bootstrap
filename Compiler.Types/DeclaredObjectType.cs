using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class DeclaredObjectType : DeclaredReferenceType
{
    public static DeclaredObjectType Create(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        bool isClass,
        string name)
        => new(containingPackage, containingNamespace, isAbstract, isConst, isClass, name,
            FixedList<GenericParameterType>.Empty, FixedSet<DeclaredObjectType>.Empty);

    public static DeclaredObjectType Create(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        bool isClass,
        string name,
        FixedList<GenericParameterType> genericParameterTypes,
        FixedSet<DeclaredObjectType> superTypes)
        => new(containingPackage, containingNamespace, isAbstract, isConst, isClass,
            StandardTypeName.Create(name, genericParameterTypes.Count), genericParameterTypes, superTypes);

    public static DeclaredObjectType Create(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        bool isClass,
        StandardTypeName name,
        FixedList<GenericParameterType> genericParametersTypes,
        FixedSet<DeclaredObjectType> superTypes)
    {
        Requires.That(nameof(genericParametersTypes), name.GenericParameterCount == genericParametersTypes.Count, "Count must match name count");
        return new(containingPackage, containingNamespace, isAbstract, isConst, isClass, name,
            genericParametersTypes, superTypes);
    }

    public static DeclaredObjectType Create(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        bool isClass,
        string name,
        params GenericParameter[] genericParameters)
    {
        var declaringTypePromise = new Promise<DeclaredObjectType>();
        var genericParametersTypes = genericParameters.Select(p => new GenericParameterType(declaringTypePromise, p)).ToFixedList();
        return new(containingPackage, containingNamespace, isAbstract, isConst, isClass,
            StandardTypeName.Create(name, genericParameters.Length), genericParametersTypes,
            FixedSet<DeclaredObjectType>.Empty);
    }

    private DeclaredObjectType(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        bool isClass,
        StandardTypeName name,
        FixedList<GenericParameterType> genericParametersTypes,
        FixedSet<DeclaredObjectType> superTypes)
        : base(isAbstract)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        IsConst = isConst;
        IsClass = isClass;
        Name = name;
        GenericParameters = genericParametersTypes.Select(t => t.Parameter).ToFixedList();
        GenericParameterTypes = genericParametersTypes;
        GenericParameterDataTypes = GenericParameterTypes.ToFixedList<DataType>();
        SuperTypes = superTypes;

        // Fulfill the declaring type promise so the parameters are associated to this type
        var declaringTypePromise = genericParametersTypes.Select(t => t.DeclaringTypePromise)
                                                         .Distinct().SingleOrDefault();
        declaringTypePromise?.Fulfill(this);
    }

    public override SimpleName ContainingPackage { get; }

    public override NamespaceName ContainingNamespace { get; }

    public override StandardTypeName Name { get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConst { get; }

    /// <summary>
    /// Whether this type is a `class` (as opposed to a `trait`)
    /// </summary>
    public bool IsClass { get; }

    public FixedList<GenericParameter> GenericParameters { get; }

    public FixedList<GenericParameterType> GenericParameterTypes { get; }

    // TODO this is really awkward. There should be a subtype relationship
    public FixedList<DataType> GenericParameterDataTypes { get; }

    public FixedSet<DeclaredObjectType> SuperTypes { get; }

    /// <summary>
    /// Make a version of this type for use as the default constructor parameter.
    /// </summary>
    /// <remarks>This is always `init mut` because the type is being initialized and can be mutated
    /// inside the constructor via field initializers.</remarks>
    public ObjectType ToDefaultConstructorSelf()
        => With(ReferenceCapability.InitMutable, GenericParameterDataTypes);

    /// <summary>
    /// Make a version of this type for use as the return type of the default constructor.
    /// </summary>
    /// <remarks>This is always either `iso` or `const` depending on whether the type was declared
    /// with `const` because there are no parameters that could break the new objects isolation.</remarks>
    public ObjectType ToDefaultConstructorReturn()
        => With(IsConst ? ReferenceCapability.Constant : ReferenceCapability.Isolated, GenericParameterDataTypes);

    /// <summary>
    /// Determine the return type of a constructor with the given parameter types.
    /// </summary>
    /// <remarks>The capability of the return type is restricted by the parameter types because the
    /// newly constructed object could contain references to them.</remarks>
    public ObjectType ToConstructorReturn(ReferenceType selfParameterType, IEnumerable<ParameterType> parameterTypes)
    {
        if (IsConst) return With(ReferenceCapability.Constant, GenericParameterDataTypes);
        // Read only self constructors cannot return `mut` or `iso`
        if (!selfParameterType.AllowsWrite)
            return With(ReferenceCapability.ReadOnly, GenericParameterDataTypes);
        foreach (var parameterType in parameterTypes)
            switch (parameterType.Type)
            {
                case ReferenceType when parameterType.IsLentBinding:
                case ReferenceType { IsConstReference: true }:
                case ReferenceType { IsIsolatedReference: true }:
                case OptionalType { Referent: ReferenceType } when parameterType.IsLentBinding:
                case OptionalType { Referent: ReferenceType { IsConstReference: true } }:
                case OptionalType { Referent: ReferenceType { IsIsolatedReference: true } }:
                case SimpleType:
                case EmptyType:
                case UnknownType:
                    continue;
                default:
                    return With(ReferenceCapability.Mutable, GenericParameterDataTypes);
            }

        return With(ReferenceCapability.Isolated, GenericParameterDataTypes);
    }

    public override ObjectType With(ReferenceCapability capability, FixedList<DataType> typeArguments)
        => ObjectType.Create(capability, this, typeArguments);

    public override bool IsAssignableFrom(DeclaredReferenceType source)
        => Equals(source)
           || (source is DeclaredObjectType sourceObjectType && sourceObjectType.SuperTypes.Contains(this));

    /// <summary>
    /// Make a version of this type that is the default read reference capability for the type. That
    /// is either read-only or constant.
    /// </summary>
    public ObjectType WithRead(FixedList<DataType> typeArguments)
        => With(IsConst ? ReferenceCapability.Constant : ReferenceCapability.ReadOnly, typeArguments);

    /// <summary>
    /// Make a version of this type that is the default mutate reference capability for the type.
    /// For constant types, that isn't allowed and a constant reference is returned.
    /// </summary>
    public ObjectType WithMutate(FixedList<DataType> typeArguments)
        => With(IsConst ? ReferenceCapability.Constant : ReferenceCapability.Mutable, typeArguments);

    #region Equals
    public override bool Equals(DeclaredReferenceType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is DeclaredObjectType objectType
            && ContainingPackage == objectType.ContainingPackage
            && ContainingNamespace == objectType.ContainingNamespace
            && IsAbstract == objectType.IsAbstract
            && IsConst == objectType.IsConst
            && Name == objectType.Name
            && GenericParameters.Equals(objectType.GenericParameters);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingPackage, ContainingNamespace, IsAbstract, IsConst, Name, GenericParameters);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder);
        return builder.ToString();
    }

    public void ToString(StringBuilder builder)
    {
        builder.Append('<');
        builder.Append(ContainingPackage);
        builder.Append(">::");
        builder.Append(ContainingNamespace);
        if (ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(Name);
        if (!GenericParameters.Any()) return;

        builder.Append('[');
        builder.AppendJoin(", ", GenericParameters);
        builder.Append(']');
    }
}
