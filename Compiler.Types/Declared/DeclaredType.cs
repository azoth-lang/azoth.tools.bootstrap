using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

[Closed(typeof(DeclaredReferenceType), typeof(DeclaredValueType))]
public abstract class DeclaredType : IEquatable<DeclaredType>
{
    #region Standard Types
    public static readonly BoolType Bool = BoolType.Instance;

    public static readonly BigIntegerType Int = BigIntegerType.Int;
    public static readonly BigIntegerType UInt = BigIntegerType.UInt;
    public static readonly FixedSizeIntegerType Int8 = FixedSizeIntegerType.Int8;
    public static readonly FixedSizeIntegerType Byte = FixedSizeIntegerType.Byte;
    public static readonly FixedSizeIntegerType Int16 = FixedSizeIntegerType.Int16;
    public static readonly FixedSizeIntegerType UInt16 = FixedSizeIntegerType.UInt16;
    public static readonly FixedSizeIntegerType Int32 = FixedSizeIntegerType.Int32;
    public static readonly FixedSizeIntegerType UInt32 = FixedSizeIntegerType.UInt32;
    public static readonly FixedSizeIntegerType Int64 = FixedSizeIntegerType.Int64;
    public static readonly FixedSizeIntegerType UInt64 = FixedSizeIntegerType.UInt64;
    public static readonly PointerSizedIntegerType Size = PointerSizedIntegerType.Size;
    public static readonly PointerSizedIntegerType Offset = PointerSizedIntegerType.Offset;
    public static readonly PointerSizedIntegerType NInt = PointerSizedIntegerType.NInt;
    public static readonly PointerSizedIntegerType NUInt = PointerSizedIntegerType.NUInt;

    public static readonly AnyType Any = AnyType.Instance;
    #endregion

    public abstract IdentifierName? ContainingPackage { get; }
    public abstract NamespaceName ContainingNamespace { get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsDeclaredConst { get; }

    public abstract TypeName Name { get; }

    public IFixedList<GenericParameter> GenericParameters { get; }
    public bool HasIndependentGenericParameters { get; }
    public bool AllowsVariance { get; }
    public IFixedList<GenericParameterType> GenericParameterTypes { get; }
    public bool IsGeneric => GenericParameters.Any();
    public abstract IFixedSet<BareReferenceType> Supertypes { get; }

    private protected DeclaredType(
        bool isDeclaredConst,
        IFixedList<GenericParameterType> genericParametersTypes)
    {
        IsDeclaredConst = isDeclaredConst;
        GenericParameters = genericParametersTypes.Select(t => t.Parameter).ToFixedList();
        HasIndependentGenericParameters = GenericParameters.Any(p => p.HasIndependence);
        AllowsVariance = GenericParameters.Any(p => p.Variance != ParameterVariance.Invariant);
        GenericParameterTypes = genericParametersTypes;
    }

    public abstract BareType With(IFixedList<DataType> typeArguments);

    public abstract CapabilityType With(Capability capability, IFixedList<DataType> typeArguments);

    /// <summary>
    /// Make a version of this type that is the default read reference capability for the type. That
    /// is either read-only or constant.
    /// </summary>
    public virtual CapabilityType WithRead(IFixedList<DataType> typeArguments)
        => With(IsDeclaredConst ? Capability.Constant : Capability.Read, typeArguments);

    #region Equality
    public abstract bool Equals(DeclaredType? other);

    public override bool Equals(object? obj) => obj is DeclaredType other && Equals(other);

    public abstract override int GetHashCode();

    public static bool operator ==(DeclaredType? left, DeclaredType? right) => Equals(left, right);

    public static bool operator !=(DeclaredType? left, DeclaredType? right) => !Equals(left, right);
    #endregion

    public abstract override string ToString();

    protected void RequiresEmpty(IFixedList<DataType> typeArguments)
    {
        if (typeArguments.Count != 0)
            throw new ArgumentException($"`{Name}` does not support type arguments.");
    }
}
