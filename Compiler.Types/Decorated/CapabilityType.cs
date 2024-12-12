using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// TODO should this cover optional types since they are implicit const?
// e.g. `mut Foo`, `const Self`, etc. when not applied to GenericParameterPlainType
// e.g. `read |> T` when applied to GenericParameterPlainType
// Cannot be applied to FunctionPlainType, NeverPlainType
// Open Questions:
// * Can it be applied to `void` in which case it must be implicit `const`?
// * Can it be applied to optional types in which case it must be implicit `const`?
// If answer to both is no, then can apply to:
// * VariablePlainType
//   * GenericParameterPlainType
//   * SelfParameterPlainType
//   * AssociatedPlainType
// * ConstructedPlainType
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
// TODO maybe this should be a wrapper around ConstructedBareType. It seems to share a lot of logic
public sealed class CapabilityType : INonVoidType
{
    public static CapabilityType Create(Capability capability, ConstructedOrAssociatedPlainType plainType)
        => new(capability, plainType, []);

    public static CapabilityType Create(
        Capability capability,
        ConstructedOrAssociatedPlainType plainType,
        IFixedList<IType> arguments)
        => new(capability, plainType, arguments);

    // TODO this seems like it should be removed
    public static IMaybeType LaxCreate(Capability capability, IMaybeType referent)
        => referent switch
        {
            GenericParameterType t => CapabilityViewpointType.Create(capability, t),
            CapabilityType t => t.AccessedVia(capability),
            VoidType t => t,
            NeverType t => t,
            _ => IType.Unknown,
        };

    public Capability Capability { get; }
    public ConstructedOrAssociatedPlainType PlainType { get; }
    NonVoidPlainType INonVoidType.PlainType => PlainType;
    IMaybePlainType IMaybeType.PlainType => PlainType;

    public TypeConstructor? TypeConstructor => PlainType.TypeConstructor;

    public IFixedList<IType> Arguments { get; }

    public bool HasIndependentTypeArguments { get; }

    public IFixedList<TypeParameterArgument> TypeParameterArguments { get; }

    public TypeReplacements TypeReplacements { get; }

    private CapabilityType(
        Capability capability,
        ConstructedOrAssociatedPlainType plainType,
        IFixedList<IType> arguments)
    {
        Requires.That(plainType.Arguments.SequenceEqual(arguments.Select(a => a.PlainType)), nameof(arguments),
            "Type arguments must match plain type.");
        Capability = capability;
        PlainType = plainType;
        Arguments = arguments;
        HasIndependentTypeArguments = (PlainType.TypeConstructor?.HasIndependentParameters ?? false)
                                      || Arguments.Any(a => a.HasIndependentTypeArguments);
        TypeParameterArguments = (PlainType.TypeConstructor?.Parameters ?? [])
                                 .EquiZip(Arguments, (p, a) => new TypeParameterArgument(p, a)).ToFixedList();
        // TODO could pass TypeParameterArguments instead?
        TypeReplacements = plainType.TypeConstructor is null ? TypeReplacements.None
            : new(plainType.TypeReplacements, plainType.TypeConstructor, Arguments);
    }

    public IType ToNonLiteral()
    {
        var newPlainType = PlainType.ToNonLiteral();
        if (ReferenceEquals(PlainType, newPlainType)) return this;
        // TODO eliminate this cast
        return new CapabilityType(Capability, (ConstructedPlainType)newPlainType, Arguments);
    }

    public CapabilityType With(Capability capability) => new(capability, PlainType, Arguments);

    public CapabilityType With(IFixedList<IType> arguments) => new(Capability, PlainType, arguments);

    public CapabilityType UpcastTo(TypeConstructor target)
    {
        if (TypeConstructor?.Equals(target) ?? false) return this;

        // TODO this will fail if the type implements the target type in multiple ways.
        var supertype = TypeConstructor?.Supertypes.Where(s => s.TypeConstructor.Equals(target)).TrySingle();
        if (supertype is null) throw new ArgumentException($"The type {target} is not a supertype of {ToILString()}.");

        var bareType = new ConstructedBareType(supertype.PlainType, supertype.Arguments);
        bareType = TypeReplacements.ReplaceTypeParametersIn(bareType);

        return bareType.With(Capability);
    }

    public bool BareTypeEquals(CapabilityType other)
    {
        if (ReferenceEquals(this, other)) return true;
        return PlainType.Equals(other.PlainType)
               && Arguments.Equals(other.Arguments);
    }

    #region Equality
    public bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilityType otherType
               && Capability.Equals(otherType.Capability)
               && PlainType.Equals(otherType.PlainType)
               && Arguments.Equals(otherType.Arguments);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is CapabilityType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Capability, PlainType, Arguments);
    #endregion

    public override string ToString() => ToILString();

    public string ToSourceCodeString()
        => ToString(Capability.ToSourceCodeString(), t => t.ToSourceCodeString());

    public string ToILString()
        => ToString(Capability.ToILString(), t => t.ToILString());

    private string ToString(string capability, Func<IType, string> toString)
    {
        var builder = new StringBuilder();
        builder.Append(capability);
        builder.Append(' ');
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
