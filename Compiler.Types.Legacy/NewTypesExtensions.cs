using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public static class NewTypesExtensions
{
    public static BareNonVariableType With(this TypeConstructor typeConstructor, IFixedList<IType> typeArguments)
        => BareNonVariableType.Create(typeConstructor, typeArguments);

    public static BareNonVariableType? With(this TypeConstructor typeConstructor, IFixedList<IMaybeType> typeArguments)
    {
        var properTypeArguments = typeArguments.As<IType>();
        if (properTypeArguments is null) return null;
        return typeConstructor.With(properTypeArguments);
    }

    public static CapabilityType With(this TypeConstructor typeConstructor, Capability capability, IFixedList<IType> typeArguments)
        => typeConstructor.With(typeArguments).With(capability);

    public static CapabilityTypeConstraint With(this TypeConstructor typeConstructor, CapabilitySet capability, IFixedList<IType> typeArguments)
        => typeConstructor.With(typeArguments).With(capability);

    /// <summary>
    /// Make a version of this type that is the default mutate reference capability for the type.
    /// For constant types, that isn't allowed and a constant reference is returned.
    /// </summary>
    public static CapabilityType WithMutate(this TypeConstructor typeConstructor, IFixedList<IType> typeArguments)
        => typeConstructor.With(typeConstructor.IsDeclaredConst ? Capability.Constant : Capability.Mutable, typeArguments);

    /// <summary>
    /// Make a version of this type that is the default read reference capability for the type. That
    /// is either read-only or constant.
    /// </summary>
    public static CapabilityType WithRead(this TypeConstructor typeConstructor, IFixedList<IType> typeArguments)
        => typeConstructor.With(typeArguments).WithRead();

    public static IMaybeType WithRead(this TypeConstructor typeConstructor, IFixedList<IMaybeType> typeArguments)
    {
        var properTypeArguments = typeArguments.As<IType>();
        if (properTypeArguments is null) return IType.Unknown;
        return typeConstructor.WithRead(properTypeArguments);
    }

    /// <summary>
    /// Make a version of this type for use as the default constructor or initializer parameter.
    /// </summary>
    /// <remarks>This is always `init mut` because the type is being initialized and can be mutated
    /// inside the constructor via field initializers.</remarks>
    public static CapabilityType ToDefaultConstructorSelf(this OrdinaryTypeConstructor typeConstructor)
        => typeConstructor.With(Capability.InitMutable, typeConstructor.GenericParameterTypes());

    /// <summary>
    /// Make a version of this type for use as the return type of the default constructor or initializer.
    /// </summary>
    /// <remarks>This is always either `iso` or `const` depending on whether the type was declared
    /// with `const` because there are no parameters that could break the new objects isolation.</remarks>
    public static CapabilityType ToDefaultConstructorReturn(this OrdinaryTypeConstructor typeConstructor)
        => typeConstructor.With(typeConstructor.IsDeclaredConst ? Capability.Constant : Capability.Isolated,
            typeConstructor.GenericParameterTypes());

    /// <summary>
    /// Determine the return type of a constructor or initializer with the given parameter types.
    /// </summary>
    /// <remarks>The capability of the return type is restricted by the parameter types because the
    /// newly constructed object could contain references to them.</remarks>
    public static CapabilityType ToConstructorReturn(this OrdinaryTypeConstructor typeConstructor, CapabilityType selfParameterType, IEnumerable<ParameterType> parameterTypes)
    {
        if (typeConstructor.IsDeclaredConst) return typeConstructor.With(Capability.Constant,
            typeConstructor.GenericParameterTypes());
        // Read only self constructors cannot return `mut` or `iso`
        if (!selfParameterType.AllowsWrite)
            return typeConstructor.With(Capability.Read, typeConstructor.GenericParameterTypes());
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
                    return typeConstructor.With(Capability.Mutable, typeConstructor.GenericParameterTypes());
            }

        return typeConstructor.With(Capability.Isolated, typeConstructor.GenericParameterTypes());
    }

    public static IFixedList<GenericParameterType> GenericParameterTypes(this TypeConstructor typeConstructor)
    {
        if (typeConstructor.ParameterPlainTypes.IsEmpty) return [];
        return typeConstructor.ParameterPlainTypes.Select(p => p.ToType()).ToFixedList();
    }

    public static IFixedList<Decorated.IType> ToDecoratedTypes(this IEnumerable<IType> types)
        => types.Select(t => t.ToDecoratedType()).ToFixedList();

    public static IFixedList<Decorated.ParameterType> ToDecoratedTypes(this IEnumerable<ParameterType> parameters)
        => parameters.Select(t => t.ToDecoratedType()).ToFixedList();

    public static IFixedList<ParameterType> ToTypes(this IEnumerable<Decorated.ParameterType> parameters)
        => parameters.Select(p => p.ToType()).ToFixedList();

    public static IFixedList<IType> ToTypes(this IEnumerable<Decorated.IType> types)
        => types.Select(p => p.ToType()).ToFixedList();

    public static ParameterType ToType(this Decorated.ParameterType parameter)
        => new(parameter.IsLent, parameter.Type.ToType());

    public static BareNonVariableType ToType(this TypeConstructor.Supertype supertype)
    {
        var args = supertype.TypeArguments.Select(arg => arg.ToType()).ToFixedList();
        return new(supertype.TypeConstructor, args);
    }

    public static IType ToType(this Decorated.IType type)
        => type switch
        {
            Decorated.INonVoidType t => t.ToType(),
            Decorated.VoidType t => IType.Void,
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public static INonVoidType ToType(this Decorated.INonVoidType type)
        => type switch
        {
            Decorated.CapabilityType t => t.ToType(),
            Decorated.FunctionType t => new FunctionType(t.Parameters.ToTypes(), t.Return.ToType()),
            Decorated.GenericParameterType t => new GenericParameterType(t.PlainType.DeclaringTypeConstructor, t.PlainType.Parameter),
            Decorated.NeverType _ => IType.Never,
            Decorated.OptionalType t => new OptionalType(t.Referent.ToType()),
            Decorated.SelfViewpointType t => SelfViewpointType.Create(t.Capability, t.Referent.ToType()),
            Decorated.CapabilitySetSelfType _ => throw new NotImplementedException("CapabilitySetSelfType would be a psuedo type"),
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public static INonVoidType ToType(this Decorated.CapabilityType type)
    {
        switch (type.PlainType)
        {
            case GenericParameterPlainType t:
                return CapabilityViewpointType.Create(type.Capability, t.ToType());
            case SelfPlainType t:
            {
                var selfType = new SelfType(t.ContainingType);
                return new CapabilityType(type.Capability, selfType);
            }
            case OrdinaryAssociatedPlainType _:
                throw new NotImplementedException();
            case ConstructedPlainType t:
            {
                var bareType = new BareNonVariableType(t.TypeConstructor, type.TypeArguments.ToTypes());
                return new CapabilityType(type.Capability, bareType);
            }
            default:
                throw ExhaustiveMatch.Failed(type.PlainType);
        }
    }

    public static GenericParameterType ToType(this GenericParameterPlainType plainType)
        => new GenericParameterType(plainType.DeclaringTypeConstructor, plainType.Parameter);

    public static SelfType ToType(this SelfPlainType plainType) => new(plainType.ContainingType);

    public static INonVoidType ToType(this ConstructedPlainType plainType)
        => plainType.TypeConstructor switch
        {
            SimpleOrLiteralTypeConstructor t => t.ToType(),
            AnyTypeConstructor _
                => throw new NotSupportedException("Conversion to type only supported on simple or literal type constructor"),
            OrdinaryTypeConstructor _
                => throw new NotSupportedException("Conversion to type only supported on simple or literal type constructor"),
            _ => throw ExhaustiveMatch.Failed(plainType.TypeConstructor),
        };

    public static INonVoidType ToType(this SimpleOrLiteralTypeConstructor typeConstructor)
        => typeConstructor switch
        {
            LiteralTypeConstructor t => t.ToType(),
            SimpleTypeConstructor t => t.ToType(),
            _ => throw ExhaustiveMatch.Failed(typeConstructor),
        };

    public static ConstValueType ToType(this LiteralTypeConstructor typeConstructor)
        => typeConstructor switch
        {
            BoolLiteralTypeConstructor t => (BoolConstValueType)t.Value,
            IntegerLiteralTypeConstructor t => new IntegerConstValueType(t.Value),
            _ => throw ExhaustiveMatch.Failed(typeConstructor),
        };

    public static CapabilityType ToType(this SimpleTypeConstructor typeConstructor)
        => typeConstructor switch
        {
            BoolTypeConstructor _ => IType.Bool,
            BigIntegerTypeConstructor t => t.IsSigned ? IType.Int : IType.UInt,
            PointerSizedIntegerTypeConstructor t => t.ToType(),
            FixedSizeIntegerTypeConstructor t => t.ToType(),
            _ => throw ExhaustiveMatch.Failed(typeConstructor),
        };

    public static CapabilityType ToType(this PointerSizedIntegerTypeConstructor typeConstructor)
    {
        if (typeConstructor.Equals(TypeConstructor.Size))
            return IType.Size;

        if (typeConstructor.Equals(TypeConstructor.Offset))
            return IType.Offset;

        if (typeConstructor.Equals(TypeConstructor.NInt))
            return IType.NInt;

        if (typeConstructor.Equals(TypeConstructor.NUInt))
            return IType.NUInt;

        throw new UnreachableException();
    }

    public static CapabilityType ToType(this FixedSizeIntegerTypeConstructor typeConstructor)
        => typeConstructor.Bits switch
        {
            8 => typeConstructor.IsSigned ? IType.Int8 : IType.Byte,
            16 => typeConstructor.IsSigned ? IType.Int16 : IType.UInt16,
            32 => typeConstructor.IsSigned ? IType.Int32 : IType.UInt32,
            64 => typeConstructor.IsSigned ? IType.Int64 : IType.UInt64,
            _ => throw new UnreachableException("Bits not an expected value"),
        };
}
