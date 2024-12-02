using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using MoreLinq.Extensions;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

/// <summary>
/// A type that could have a reference capability applied, but without a reference capability.
/// </summary>
[Closed(typeof(BareReferenceType), typeof(BareValueType))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class BareNonVariableType : BareType
{
    #region Standard Types
    public static readonly BareValueType<BoolType> Bool = DeclaredType.Bool.BareType;

    public static readonly BareValueType<BigIntegerType> Int = DeclaredType.Int.BareType;
    public static readonly BareValueType<BigIntegerType> UInt = DeclaredType.UInt.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> Int8 = DeclaredType.Int8.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> Byte = DeclaredType.Byte.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> Int16 = DeclaredType.Int16.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> UInt16 = DeclaredType.UInt16.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> Int32 = DeclaredType.Int32.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> UInt32 = DeclaredType.UInt32.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> Int64 = DeclaredType.Int64.BareType;
    public static readonly BareValueType<FixedSizeIntegerType> UInt64 = DeclaredType.UInt64.BareType;
    public static readonly BareValueType<PointerSizedIntegerType> Size = DeclaredType.Size.BareType;
    public static readonly BareValueType<PointerSizedIntegerType> Offset = DeclaredType.Offset.BareType;

    public static readonly BareReferenceType<AnyType> Any = DeclaredType.Any.BareType;
    #endregion

    public abstract override DeclaredType DeclaredType { get; }
    public sealed override TypeName Name => DeclaredType.Name;
    public sealed override IFixedList<IType> GenericTypeArguments { get; }
    public sealed override IEnumerable<GenericParameterArgument> GenericParameterArguments
        => DeclaredType.GenericParameters.EquiZip(GenericTypeArguments, (p, a) => new GenericParameterArgument(p, a));
    public bool AllowsVariance { get; }
    public bool HasIndependentTypeArguments { get; }

    private readonly Lazy<IFixedSet<BareReferenceType>> supertypes;
    public IFixedSet<BareReferenceType> Supertypes => supertypes.Value;

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsDeclaredConst => DeclaredType.IsDeclaredConst;

    private readonly Lazy<TypeReplacements> typeReplacements;

    public static BareReferenceType<ObjectType> Create(
        ObjectType declaredType,
        IFixedList<IType> typeArguments)
        => new(declaredType, typeArguments);

    public static BareValueType<StructType> Create(
        StructType declaredType,
        IFixedList<IType> typeArguments)
        => new(declaredType, typeArguments);

    private protected BareNonVariableType(DeclaredType declaredType, IFixedList<IType> genericTypeArguments)
    {
        if (declaredType.GenericParameters.Count != genericTypeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match. Given `[{genericTypeArguments.ToILString()}]` for `{declaredType}`.",
                nameof(genericTypeArguments));
        GenericTypeArguments = genericTypeArguments;
        AllowsVariance = declaredType.AllowsVariance
            || GenericTypeArguments.Any(a => a.AllowsVariance);
        HasIndependentTypeArguments = declaredType.HasIndependentGenericParameters
                                      || GenericTypeArguments.Any(a => a.HasIndependentTypeArguments);

        typeReplacements = new(GetTypeReplacements);
        supertypes = new(GetSupertypes);
    }

    public INonVoidPlainType ToPlainType()
    {
        var typeArguments = GenericTypeArguments.Select(a => a.ToPlainType()).ToFixedList();
        // The ToTypeConstructor() should never result in void since DeclaredType can't be void.
        return (INonVoidPlainType)(DeclaredType.TryToPlainType()
                                  ?? DeclaredType.ToTypeConstructor()?.Construct(typeArguments))!;
    }

    private TypeReplacements GetTypeReplacements() => new(DeclaredType, GenericTypeArguments);

    private IFixedSet<BareReferenceType> GetSupertypes()
        => DeclaredType.Supertypes.Select(typeReplacements.Value.ReplaceTypeParametersIn).ToFixedSet();

    public IType ReplaceTypeParametersIn(IType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public IMaybeType ReplaceTypeParametersIn(IMaybeType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public IExpressionType ReplaceTypeParametersIn(IExpressionType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public IMaybeExpressionType ReplaceTypeParametersIn(IMaybeExpressionType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public BareReferenceType ReplaceTypeParametersIn(BareReferenceType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public IPseudotype ReplaceTypeParametersIn(IPseudotype pseudotype)
        => typeReplacements.Value.ReplaceTypeParametersIn(pseudotype);

    public IMaybePseudotype ReplaceTypeParametersIn(IMaybePseudotype pseudotype)
        => typeReplacements.Value.ReplaceTypeParametersIn(pseudotype);

    public abstract BareNonVariableType AccessedVia(Capability capability);

    protected IFixedList<IType> TypeArgumentsAccessedVia(Capability capability)
    {
        var newTypeArguments = new List<IType>(GenericTypeArguments.Count);
        var typesReplaced = false;
        foreach (var arg in GenericTypeArguments)
        {
            var newTypeArg = arg.AccessedVia(capability);
            typesReplaced |= !ReferenceEquals(newTypeArg, arg);
            newTypeArguments.Add(newTypeArg);
        }
        return typesReplaced ? newTypeArguments.ToFixedList() : GenericTypeArguments;
    }

    public abstract BareNonVariableType With(IFixedList<IType> typeArguments);

    public CapabilityTypeConstraint With(CapabilitySet capability)
        => new(capability, this);

    public sealed override CapabilityType WithRead()
        => With(IsDeclaredConst ? Capability.Constant : Capability.Read);

    public sealed override string ToSourceCodeString() => ToString(t => t.ToSourceCodeString());
    public sealed override string ToILString() => ToString(t => t.ToILString());

    private string ToString(Func<IType, string> toString)
    {
        var builder = new StringBuilder();
        builder.Append(DeclaredType.ContainingNamespace);
        if (DeclaredType.ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(DeclaredType.Name.ToBareString());
        if (!GenericTypeArguments.IsEmpty)
        {
            builder.Append('[');
            builder.AppendJoin(", ", GenericTypeArguments.Select(toString));
            builder.Append(']');
        }
        return builder.ToString();
    }
}
