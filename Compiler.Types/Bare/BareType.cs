using System.Diagnostics;
using System.Text;
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
    public static readonly BareType Any = new(AnyTypeConstructor.PlainType, []);
    public static readonly IFixedSet<BareType> AnySet = Any.Yield().ToFixedSet();

    // TODO add containing type for capabilities on containing type arguments

    public ConstructedPlainType PlainType { get; }
    public TypeConstructor TypeConstructor => PlainType.TypeConstructor;
    public IFixedList<Type> Arguments { get; }
    public bool HasIndependentTypeArguments { get; }
    public BareTypeReplacements TypeReplacements { get; }

    public IFixedList<TypeParameterArgument> TypeParameterArguments
        => Lazy.Initialize(ref typeParameterArguments, PlainType, Arguments,
            static (t, args) => t.TypeConstructor.Parameters
                           .EquiZip(args,
                               (p, a) => new TypeParameterArgument(p, a)).ToFixedList());
    private IFixedList<TypeParameterArgument>? typeParameterArguments;

    public IFixedSet<BareType> Supertypes
        => Lazy.Initialize(ref supertypes, TypeConstructor, TypeReplacements,
            static (constructor, replacements)
                => constructor.Supertypes.Select(replacements.Apply).ToFixedSet());
    private IFixedSet<BareType>? supertypes;

    public BareType(ConstructedPlainType plainType, IFixedList<Type> arguments)
    {
        Requires.That(plainType.Arguments.SequenceEqual(arguments.Select(a => a.PlainType)), nameof(arguments),
            "Type arguments must match plain type.");
        PlainType = plainType;
        Arguments = arguments;
        HasIndependentTypeArguments = PlainType.TypeConstructor.HasIndependentParameters
                                      || Arguments.Any(a => a.HasIndependentTypeArguments);
        // TODO could pass TypeParameterArguments instead?
        TypeReplacements = new(plainType.TypeReplacements, plainType.TypeConstructor, Arguments);
    }

    public CapabilityType With(Capability capability)
        => CapabilityType.Create(capability, this);

    /// <summary>
    /// This type with whatever the default read capability is for the type based on whether it is
    /// declared `const`.
    /// </summary>
    public CapabilityType WithDefaultCapability()
        => With(TypeConstructor.IsDeclaredConst ? Capability.Constant : Capability.Read);

    /// <summary>
    /// This type with whatever the default mutable capability is for the type based on whether it is
    /// declared `const`.
    /// </summary>
    // TODO remove method
    public CapabilityType WithDefaultMutate()
        => With(TypeConstructor.IsDeclaredConst ? Capability.Constant : Capability.Mutable);

    public BareType WithReplacement(IFixedList<Type> arguments)
        => new BareType(PlainType, arguments);

    public BareType? TryToNonLiteral()
    {
        var newPlainType = PlainType.TryToNonLiteral();
        if (newPlainType is null) return null;
        return new(newPlainType, Arguments);
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
