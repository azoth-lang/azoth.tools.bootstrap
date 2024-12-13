using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// A bare type that is a constructed type.
/// </summary>
/// <remarks>Keeping this separate from <see cref="BareType"/> enables it to be used as the type for
/// supertypes. This encodes the fact that <see cref="AssociatedBareType"/>s are not allowed to be
/// used as supertypes.</remarks>
public sealed class ConstructedBareType : BareType
{
    public override ConstructedPlainType PlainType { get; }
    public override TypeConstructor TypeConstructor => PlainType.TypeConstructor;
    public override IFixedList<IType> Arguments { get; }
    public override bool HasIndependentTypeArguments { get; }

    public override TypeReplacements TypeReplacements { get; }

    public override IFixedList<TypeParameterArgument> TypeParameterArguments
        => LazyInitializer.EnsureInitialized(ref typeParameterArguments,
            () => PlainType.TypeConstructor.Parameters
                           .EquiZip(Arguments,
                               (p, a) => new TypeParameterArgument(p, a)).ToFixedList());
    private IFixedList<TypeParameterArgument>? typeParameterArguments;

    public IFixedSet<ConstructedBareType> Supertypes
        => LazyInitializer.EnsureInitialized(ref supertypes,
            () => TypeConstructor.Supertypes.Select(t => TypeReplacements.ReplaceTypeParametersIn(t)).ToFixedSet());
    private IFixedSet<ConstructedBareType>? supertypes;

    public ConstructedBareType(ConstructedPlainType plainType, IFixedList<IType> arguments)
    {
        Requires.That(plainType.Arguments.SequenceEqual(arguments.Select(a => a.PlainType)),
            nameof(arguments), "Type arguments must match plain type.");
        PlainType = plainType;
        Arguments = arguments;
        HasIndependentTypeArguments = PlainType.TypeConstructor.HasIndependentParameters
                                      || Arguments.Any(a => a.HasIndependentTypeArguments);
        // TODO could pass TypeParameterArguments instead?
        TypeReplacements = new(plainType.TypeReplacements, plainType.TypeConstructor, Arguments);
    }

    public override BareType With(IFixedList<IType> arguments)
        => new ConstructedBareType(PlainType, arguments);

    public override ConstructedBareType? TryToNonLiteral()
    {
        var newPlainType = PlainType.TryToNonLiteral();
        if (newPlainType is null) return null;
        return new(newPlainType, Arguments);
    }

    #region Equality
    public override bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ConstructedBareType otherType
               && PlainType.Equals(otherType.PlainType)
               && Arguments.Equals(otherType.Arguments);
    }

    public override int GetHashCode() => HashCode.Combine(PlainType, Arguments);
    #endregion

    public override string ToSourceCodeString() => ToString(t => t.ToSourceCodeString());

    public override string ToILString() => ToString(t => t.ToILString());

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
