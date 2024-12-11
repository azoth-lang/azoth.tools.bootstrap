using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

// TODO consider eliminating by making BareType a class over ConstructedOrVariablePlainType
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class ConstructedBareType : BareType
{
    public ConstructedPlainType PlainType { get; }
    ConstructedOrVariablePlainType BareType.PlainType => PlainType;
    public TypeConstructor TypeConstructor => PlainType.TypeConstructor;
    public IFixedList<IType> Arguments { get; }

    public TypeReplacements TypeReplacements { get; }

    public IFixedList<TypeParameterArgument> TypeParameterArguments
        => LazyInitializer.EnsureInitialized(ref typeParameterArguments,
            () => PlainType.TypeConstructor.Parameters
                           .EquiZip(Arguments,
                               (p, a) => new TypeParameterArgument(p, a)).ToFixedList());
    private IFixedList<TypeParameterArgument>? typeParameterArguments;

    public IFixedSet<ConstructedBareType> Supertypes
        => LazyInitializer.EnsureInitialized(ref supertypes,
            () => TypeConstructor.Supertypes.Select(t => TypeReplacements.ReplaceTypeParametersIn(t).ToBareType()).ToFixedSet());
    private IFixedSet<ConstructedBareType>? supertypes;

    public ConstructedBareType(ConstructedPlainType plainType, IFixedList<IType> arguments)
    {
        Requires.That(plainType.Arguments.SequenceEqual(arguments.Select(a => a.PlainType)),
            nameof(arguments), "Type arguments must match plain type.");
        PlainType = plainType;
        Arguments = arguments;
        TypeReplacements = new(plainType.TypeReplacements, plainType.TypeConstructor, Arguments);
    }

    public CapabilityType With(Capability capability)
        => CapabilityType.Create(capability, PlainType, Arguments);

    public CapabilityType WithRead()
        => With(TypeConstructor.IsDeclaredConst ? Capability.Constant : Capability.Read);

    public CapabilityType WithMutate()
        => With(TypeConstructor.IsDeclaredConst ? Capability.Constant : Capability.Mutable);

    public TypeConstructor.Supertype ToSupertype() => new(PlainType, Arguments);

    #region Equality
    public bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ConstructedBareType otherType
               && PlainType.Equals(otherType.PlainType)
               && Arguments.Equals(otherType.Arguments);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is BareType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(PlainType, Arguments);

    #endregion


    public string ToSourceCodeString() => ToString(t => t.ToSourceCodeString());

    public string ToILString() => ToString(t => t.ToILString());

    private string ToString(Func<IType, string> toString)
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
