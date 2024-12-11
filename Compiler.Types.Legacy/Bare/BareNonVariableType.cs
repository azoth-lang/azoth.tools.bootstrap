using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
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
    public static readonly BareNonVariableType Bool = new(TypeConstructor.Bool, []);

    public static readonly BareNonVariableType True = new(TypeConstructor.True, []);
    public static readonly BareNonVariableType False = new(TypeConstructor.False, []);

    public static readonly BareNonVariableType Int = new(TypeConstructor.Int, []);
    public static readonly BareNonVariableType UInt = new(TypeConstructor.UInt, []);
    public static readonly BareNonVariableType Int8 = new(TypeConstructor.Int8, []);
    public static readonly BareNonVariableType Byte = new(TypeConstructor.Byte, []);
    public static readonly BareNonVariableType Int16 = new(TypeConstructor.Int16, []);
    public static readonly BareNonVariableType UInt16 = new(TypeConstructor.UInt16, []);
    public static readonly BareNonVariableType Int32 = new(TypeConstructor.Int32, []);
    public static readonly BareNonVariableType UInt32 = new(TypeConstructor.UInt32, []);
    public static readonly BareNonVariableType Int64 = new(TypeConstructor.Int64, []);
    public static readonly BareNonVariableType UInt64 = new(TypeConstructor.UInt64, []);
    public static readonly BareNonVariableType Size = new(TypeConstructor.Size, []);
    public static readonly BareNonVariableType Offset = new(TypeConstructor.Offset, []);
    public static readonly BareNonVariableType NInt = new(TypeConstructor.NInt, []);
    public static readonly BareNonVariableType NUInt = new(TypeConstructor.NUInt, []);

    public static readonly BareNonVariableType Any = new(TypeConstructor.Any, []);
    #endregion

    public override TypeConstructor TypeConstructor { get; }
    public override TypeName Name => TypeConstructor.Name;
    public override IFixedList<IType> TypeArguments { get; }
    public override IEnumerable<GenericParameterArgument> GenericParameterArguments
        => TypeConstructor.Parameters.EquiZip(TypeArguments, (p, a) => new GenericParameterArgument(p, a));
    public override bool AllowsVariance { get; }
    public override bool HasIndependentTypeArguments { get; }

    private readonly Lazy<IFixedSet<BareNonVariableType>> supertypes;
    public override IFixedSet<BareNonVariableType> Supertypes => supertypes.Value;

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public override bool IsDeclaredConst => TypeConstructor.IsDeclaredConst;

    private readonly Lazy<TypeReplacements> typeReplacements;

    public static BareNonVariableType Create(
        TypeConstructor typeConstructor,
        IFixedList<IType> typeArguments)
        => new(typeConstructor, typeArguments);

    internal BareNonVariableType(TypeConstructor typeConstructor, IFixedList<IType> typeArguments)
    {
        if (typeConstructor.Parameters.Count != typeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match. Given `[{typeArguments.ToILString()}]` for `{typeConstructor}`.",
                nameof(typeArguments));
        TypeConstructor = typeConstructor;
        TypeArguments = typeArguments;
        AllowsVariance = typeConstructor.AllowsVariance
            || TypeArguments.Any(a => a.AllowsVariance);
        HasIndependentTypeArguments = typeConstructor.HasIndependentParameters
                                      || TypeArguments.Any(a => a.HasIndependentTypeArguments);

        typeReplacements = new(GetTypeReplacements);
        supertypes = new(GetSupertypes);
    }

    public override ConstructedPlainType ToPlainType()
    {
        var typeArguments = TypeArguments.Select(a => a.ToPlainType()).ToFixedList();
        // Should never result in void since TypeConstructor can't be void.
        return TypeConstructor.Construct(typeArguments)!;
    }

    public TypeConstructor.Supertype ToSupertype()
    {
        var typeArguments = TypeArguments.Select(a => a.ToDecoratedType()).ToFixedList();
        return new(ToPlainType(), typeArguments);
    }

    private TypeReplacements GetTypeReplacements() => new(TypeConstructor, TypeArguments);

    private IFixedSet<BareNonVariableType> GetSupertypes()
        => TypeConstructor.Supertypes.Select(t => typeReplacements.Value.ReplaceTypeParametersIn(t.ToType())).ToFixedSet();

    public override IType ReplaceTypeParametersIn(IType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public override IMaybeType ReplaceTypeParametersIn(IMaybeType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public BareNonVariableType ReplaceTypeParametersIn(BareNonVariableType type)
        => typeReplacements.Value.ReplaceTypeParametersIn(type);

    public override IPseudotype ReplaceTypeParametersIn(IPseudotype pseudotype)
        => typeReplacements.Value.ReplaceTypeParametersIn(pseudotype);

    public override IMaybePseudotype ReplaceTypeParametersIn(IMaybePseudotype pseudotype)
        => typeReplacements.Value.ReplaceTypeParametersIn(pseudotype);

    public override BareNonVariableType AccessedVia(Capability capability)
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

    public override BareNonVariableType With(IFixedList<IType> typeArguments)
        => new(TypeConstructor, typeArguments);

    public CapabilityTypeConstraint With(CapabilitySet capability)
        => new(capability, this);

    public override CapabilityType With(Capability capability)
        => new(capability, this);

    public override CapabilityType WithRead()
        => With(IsDeclaredConst ? Capability.Constant : Capability.Read);

    #region Equality
    public override bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BareNonVariableType otherType
               && TypeConstructor.Equals(otherType.TypeConstructor)
               && TypeArguments.Equals(otherType.TypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(TypeConstructor, TypeArguments);
    #endregion

    public sealed override string ToSourceCodeString() => ToString(t => t.ToSourceCodeString());
    public sealed override string ToILString() => ToString(t => t.ToILString());

    private string ToString(Func<IType, string> toString)
    {
        var builder = new StringBuilder();
        TypeConstructor.Context.AppendContextPrefix(builder);
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
