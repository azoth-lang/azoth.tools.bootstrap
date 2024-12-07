using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq.Extensions;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

/// <summary>
/// A type that could have a reference capability applied, but without a reference capability.
/// </summary>
public sealed class BareNonVariableType : BareType
{
    #region Standard Types
    public static readonly BareNonVariableType Bool = DeclaredType.Bool.BareType;

    public static readonly BareNonVariableType Int = DeclaredType.Int.BareType;
    public static readonly BareNonVariableType UInt = DeclaredType.UInt.BareType;
    public static readonly BareNonVariableType Int8 = DeclaredType.Int8.BareType;
    public static readonly BareNonVariableType Byte = DeclaredType.Byte.BareType;
    public static readonly BareNonVariableType Int16 = DeclaredType.Int16.BareType;
    public static readonly BareNonVariableType UInt16 = DeclaredType.UInt16.BareType;
    public static readonly BareNonVariableType Int32 = DeclaredType.Int32.BareType;
    public static readonly BareNonVariableType UInt32 = DeclaredType.UInt32.BareType;
    public static readonly BareNonVariableType Int64 = DeclaredType.Int64.BareType;
    public static readonly BareNonVariableType UInt64 = DeclaredType.UInt64.BareType;
    public static readonly BareNonVariableType Size = DeclaredType.Size.BareType;
    public static readonly BareNonVariableType Offset = DeclaredType.Offset.BareType;

    public static readonly BareNonVariableType Any = DeclaredType.Any.BareType;
    #endregion

    public override DeclaredType TypeConstructor { get; }
    public override TypeName Name => TypeConstructor.Name;
    public override IFixedList<IType> TypeArguments { get; }
    public override IEnumerable<GenericParameterArgument> GenericParameterArguments
        => TypeConstructor.GenericParameters.EquiZip(TypeArguments, (p, a) => new GenericParameterArgument(p, a));
    public bool AllowsVariance { get; }
    public bool HasIndependentTypeArguments { get; }

    private readonly Lazy<IFixedSet<BareNonVariableType>> supertypes;
    public IFixedSet<BareNonVariableType> Supertypes => supertypes.Value;

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsDeclaredConst => TypeConstructor.IsDeclaredConst;

    private readonly Lazy<TypeReplacements> typeReplacements;

    public static BareNonVariableType Create(
        ObjectType declaredType,
        IFixedList<IType> typeArguments)
        => new(declaredType, typeArguments);

    public static BareNonVariableType Create(
        StructType declaredType,
        IFixedList<IType> typeArguments)
        => new(declaredType, typeArguments);

    internal BareNonVariableType(DeclaredType declaredType, IFixedList<IType> typeArguments)
    {
        if (declaredType.GenericParameters.Count != typeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match. Given `[{typeArguments.ToILString()}]` for `{declaredType}`.",
                nameof(typeArguments));
        TypeConstructor = declaredType;
        TypeArguments = typeArguments;
        AllowsVariance = declaredType.AllowsVariance
            || TypeArguments.Any(a => a.AllowsVariance);
        HasIndependentTypeArguments = declaredType.HasIndependentGenericParameters
                                      || TypeArguments.Any(a => a.HasIndependentTypeArguments);

        typeReplacements = new(GetTypeReplacements);
        supertypes = new(GetSupertypes);
    }

    public INonVoidPlainType ToPlainType()
    {
        var typeArguments = TypeArguments.Select(a => a.ToPlainType()).ToFixedList();
        // The ToTypeConstructor() should never result in void since DeclaredType can't be void.
        return (INonVoidPlainType)(TypeConstructor.TryToPlainType()
                                  ?? TypeConstructor.ToTypeConstructor()?.Construct(typeArguments))!;
    }

    private TypeReplacements GetTypeReplacements() => new(TypeConstructor, TypeArguments);

    private IFixedSet<BareNonVariableType> GetSupertypes()
        => TypeConstructor.Supertypes.Select(typeReplacements.Value.ReplaceTypeParametersIn).ToFixedSet();

    public IType ReplaceTypeParametersIn(IType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public IMaybeType ReplaceTypeParametersIn(IMaybeType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public BareNonVariableType ReplaceTypeParametersIn(BareNonVariableType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public IPseudotype ReplaceTypeParametersIn(IPseudotype pseudotype)
        => typeReplacements.Value.ReplaceTypeParametersIn(pseudotype);

    public IMaybePseudotype ReplaceTypeParametersIn(IMaybePseudotype pseudotype)
        => typeReplacements.Value.ReplaceTypeParametersIn(pseudotype);

    public BareNonVariableType AccessedVia(Capability capability)
    {
        if (!HasIndependentTypeArguments) return this;
        var newTypeArguments = TypeArgumentsAccessedVia(capability);
        if (ReferenceEquals(newTypeArguments, TypeArguments)) return this;
        return new(TypeConstructor, newTypeArguments);
    }

    private IFixedList<IType> TypeArgumentsAccessedVia(Capability capability)
    {
        var newTypeArguments = new List<IType>(TypeArguments.Count);
        var typesReplaced = false;
        foreach (var arg in TypeArguments)
        {
            var newTypeArg = arg.AccessedVia(capability);
            typesReplaced |= !ReferenceEquals(newTypeArg, arg);
            newTypeArguments.Add(newTypeArg);
        }
        return typesReplaced ? newTypeArguments.ToFixedList() : TypeArguments;
    }

    public BareNonVariableType With(IFixedList<IType> typeArguments)
        => new(TypeConstructor, typeArguments);

    public CapabilityTypeConstraint With(CapabilitySet capability)
        => new(capability, this);

    public override CapabilityType With(Capability capability)
        => CapabilityType.Create(capability, this);

    public override CapabilityType WithRead()
        => With(IsDeclaredConst ? Capability.Constant : Capability.Read);

    #region Equality
    public override bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BareNonVariableType otherType
               && TypeConstructor == otherType.TypeConstructor
               && TypeArguments.Equals(otherType.TypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(TypeConstructor, TypeArguments);
    #endregion

    public sealed override string ToSourceCodeString() => ToString(t => t.ToSourceCodeString());
    public sealed override string ToILString() => ToString(t => t.ToILString());

    private string ToString(Func<IType, string> toString)
    {
        var builder = new StringBuilder();
        builder.Append(TypeConstructor.ContainingNamespace);
        if (TypeConstructor.ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(TypeConstructor.Name.ToBareString());
        if (!TypeArguments.IsEmpty)
        {
            builder.Append('[');
            builder.AppendJoin(", ", TypeArguments.Select(toString));
            builder.Append(']');
        }
        return builder.ToString();
    }
}
