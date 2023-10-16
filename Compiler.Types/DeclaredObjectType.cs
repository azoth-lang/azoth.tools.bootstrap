using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class DeclaredObjectType : DeclaredReferenceType
{
    public static DeclaredObjectType Create(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        string name,
        bool isConst)
        => new(containingPackage, containingNamespace, name, isConst, FixedList<GenericParameter>.Empty, FixedSet<DeclaredObjectType>.Empty);

    public static DeclaredObjectType Create(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        string name,
        bool isConst,
        FixedList<GenericParameter> genericParameters,
        FixedSet<DeclaredObjectType> superTypes)
        => new(containingPackage, containingNamespace, StandardTypeName.Create(name, genericParameters.Count), isConst, genericParameters, superTypes);

    public static DeclaredObjectType Create(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        StandardTypeName name,
        bool isConst,
        FixedList<GenericParameter> genericParameters,
        FixedSet<DeclaredObjectType> superTypes)
    {
        Requires.That(nameof(genericParameters), name.GenericParameterCount == genericParameters.Count, "Count must match name count");
        return new(containingPackage, containingNamespace, name, isConst, genericParameters, superTypes);
    }


    public static DeclaredObjectType Create(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        string name,
        bool isConst,
        params GenericParameter[] genericParameters)
        => new(containingPackage, containingNamespace, StandardTypeName.Create(name, genericParameters.Length), isConst, FixedList.Create(genericParameters), FixedSet<DeclaredObjectType>.Empty);

    private DeclaredObjectType(
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        StandardTypeName name,
        bool isConst,
        FixedList<GenericParameter> genericParameters,
        FixedSet<DeclaredObjectType> superTypes)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        Name = name;
        IsConst = isConst;
        GenericParameters = genericParameters;
        GenericParameterTypes = GenericParameters.Select(p => new GenericParameterType(this, p)).ToFixedList();
        GenericParameterDataTypes = GenericParameterTypes.ToFixedList<DataType>();
        SuperTypes = superTypes;
    }

    public override SimpleName ContainingPackage { get; }

    public override NamespaceName ContainingNamespace { get; }

    public override StandardTypeName Name { get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConst { get; }

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
            && Name == objectType.Name
            && IsConst == objectType.IsConst
            && GenericParameters.Equals(objectType.GenericParameters);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingPackage, ContainingNamespace, Name, IsConst, GenericParameters);
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
