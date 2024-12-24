using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// A bare type is a type that does not have a capability but could accept one.
/// </summary>
/// <remarks>While bare types do not have a capability prefix, they do have capabilities on their
/// type arguments. That is how they are distinct from plain types.</remarks>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class BareType : IEquatable<BareType>
{
    // Note: must use AnyTypeConstructor.PlainType instead of PlainType.Any to avoid circular
    // dependency when initializing statics.
    public static readonly BareType Any = new(AnyTypeConstructor.PlainType, containingType: null, []);
    public static readonly IFixedSet<BareType> AnySet = Any.Yield().ToFixedSet();

    public ConstructedPlainType PlainType { [DebuggerStepThrough] get; }
    public BareTypeConstructor TypeConstructor => PlainType.TypeConstructor;
    public BareType? ContainingType { [DebuggerStepThrough] get; }
    public IFixedList<Type> Arguments { [DebuggerStepThrough] get; }
    public bool HasIndependentTypeArguments { [DebuggerStepThrough] get; }
    public GenericParameterTypeReplacements TypeReplacements
        => Lazy.Initialize(ref bareTypeReplacements, this, static bareType => new(bareType));
    private GenericParameterTypeReplacements? bareTypeReplacements;

    public IFixedList<TypeParameterArgument> TypeParameterArguments
        => Lazy.Initialize(ref typeParameterArguments, PlainType, Arguments,
            static (t, args) => t.TypeConstructor.Parameters
                           .EquiZip(args,
                               (p, a) => new TypeParameterArgument(p, a)).ToFixedList());
    private IFixedList<TypeParameterArgument>? typeParameterArguments;

    public IFixedSet<BareType> Supertypes
        => Lazy.Initialize(ref supertypes, TypeConstructor, TypeReplacements,
            static (constructor, replacements)
                => constructor.Supertypes.Select(t => replacements.Apply(t)).ToFixedSet());
    private IFixedSet<BareType>? supertypes;

    public BareType(ConstructedPlainType plainType, BareType? containingType, IFixedList<Type> arguments)
    {
        Requires.That(Equals(plainType.ContainingType, containingType?.PlainType), nameof(containingType),
            "Must match the plain type.");
        Requires.That(plainType.Arguments.SequenceEqual(arguments.Select(a => a.PlainType)), nameof(arguments),
            "Type arguments must match plain type.");
        PlainType = plainType;
        ContainingType = containingType;
        Arguments = arguments;
        HasIndependentTypeArguments = PlainType.TypeConstructor.HasIndependentParameters
                                      || Arguments.Any(a => a.HasIndependentTypeArguments);
    }

    public CapabilityType With(Capability capability)
        => CapabilityType.Create(capability, this);

    public CapabilityType With(DeclaredCapability capability)
        => CapabilityType.Create(capability.ToCapabilityFor(TypeConstructor), this);

    /// <summary>
    /// Create a capability type with a capability appropriately modified to fit whether this type
    /// is declared constant.
    /// </summary>
    // TODO this needs a better name
    public CapabilityType WithModified(Capability capability)
        => CapabilityType.Create(capability.ToCapabilityFor(TypeConstructor), this);

    /// <summary>
    /// This type with whatever the default read capability is for the type based on whether it is
    /// declared `const`.
    /// </summary>
    public CapabilityType WithDefaultCapability()
        => With(TypeConstructor.DefaultCapability);

    public BareType WithReplacement(IFixedList<Type> arguments)
        => new BareType(PlainType, ContainingType, arguments);

    public BareType? TryToNonLiteral()
    {
        var newPlainType = PlainType.TryToNonLiteral();
        if (newPlainType is null) return null;
        return new(newPlainType, containingType: null, Arguments);
    }

    #region Equality
    public bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BareType otherType
               && PlainType.Equals(otherType.PlainType)
               && Arguments.Equals(otherType.Arguments);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is BareType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(PlainType, Arguments);
    #endregion

    public override string ToString() => ToILString();

    public string ToSourceCodeString() => ToString(t => t.ToSourceCodeString());
    public string ToILString() => ToString(t => t.ToILString());

    private string ToString(Func<Type, string> toString)
    {
        var builder = new StringBuilder();
        builder.Append(PlainType.ToBareString());
        if (!Arguments.IsEmpty)
        {
            builder.Append('[');
            builder.AppendJoin(", ", Arguments.Select(toString));
            builder.Append(']');
        }

        return builder.ToString();
    }
}
