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
        Name containingPackage,
        NamespaceName containingNamespace,
        Name name,
        bool isConst)
        => new(containingPackage, containingNamespace, name, isConst, FixedList<GenericParameter>.Empty);

    public static DeclaredObjectType Create(
        Name containingPackage,
        NamespaceName containingNamespace,
        Name name,
        bool isConst,
        FixedList<GenericParameter> genericParameters)
        => new(containingPackage, containingNamespace, name, isConst, genericParameters);

    public static DeclaredObjectType Create(
        Name containingPackage,
        NamespaceName containingNamespace,
        Name name,
        bool isConst,
        params GenericParameter[] genericParameters) =>
        new(containingPackage, containingNamespace, name, isConst, FixedList.Create(genericParameters));

    private DeclaredObjectType(
        Name containingPackage,
        NamespaceName containingNamespace,
        Name name,
        bool isConst,
        FixedList<GenericParameter> genericParameters)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        Name = name;
        IsConst = isConst;
        GenericParameters = genericParameters;
        GenericParameterTypes = GenericParameters.Select(p => new GenericParameterType(this, p)).ToFixedList();
        GenericParameterDataTypes = GenericParameterTypes.ToFixedList<DataType>();
    }

    public Name ContainingPackage { get; }

    public NamespaceName ContainingNamespace { get; }

    public override Name Name { get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConst { get; }

    public FixedList<GenericParameter> GenericParameters { get; }

    public FixedList<GenericParameterType> GenericParameterTypes { get; }

    // TODO this is really awkward. There should be a subtype relationship
    public FixedList<DataType> GenericParameterDataTypes { get; }

    /// <summary>
    /// Make a version of this type for use as the constructor parameter. One issue is
    /// that it should be mutable even if the underlying type is declared immutable.
    /// </summary>
    /// <remarks>This is always `mut` because the type can be mutated inside the constructor.</remarks>
    public ObjectType ToConstructorSelf()
        // TODO does this need to be `init`?
        => With(ReferenceCapability.Mutable, GenericParameterDataTypes);

    /// <summary>
    /// Make a version of this type for use as the return type of the default constructor.
    /// </summary>
    /// <remarks>This is always either `iso` or `const` depending on whether the type was declared
    /// with `const` because there are no parameters that could break the new objects isolation.</remarks>
    public ObjectType ToDefaultConstructorReturn()
        => With(IsConst ? ReferenceCapability.Constant : ReferenceCapability.Isolated, GenericParameterDataTypes);

    public ObjectType ToConstructorReturn(IEnumerable<DataType> parameterTypes)
    {
        if (IsConst) return With(ReferenceCapability.Constant, GenericParameterDataTypes);
        foreach (var parameterType in parameterTypes)
            switch (parameterType)
            {
                case ReferenceType { IsConstReference: true }:
                case OptionalType { Referent: ReferenceType { IsConstReference: true } }:
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

    #region Equals
    public override bool Equals(DeclaredReferenceType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is DeclaredObjectType objectType
            && ContainingPackage == objectType.ContainingPackage
            && ContainingNamespace == objectType.ContainingNamespace
            && Name == objectType.Name
            && IsConst == objectType.IsConst;
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingPackage, ContainingNamespace, Name, IsConst);
    #endregion

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
        => With(IsConst ? ReferenceCapability.Constant : ReferenceCapability.ReadOnly, typeArguments);

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
