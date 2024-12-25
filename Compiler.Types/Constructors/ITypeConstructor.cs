using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// A fully generalized type constructor capable of constructing any type.
/// </summary>
[Closed(
    typeof(BareTypeConstructor),
    typeof(VoidTypeConstructor),
    typeof(NeverTypeConstructor),
    typeof(GenericParameterTypeConstructor))]
public interface ITypeConstructor
{
    #region Standard
    public static readonly ITypeConstructor Void = VoidTypeConstructor.Instance;
    public static readonly ITypeConstructor Never = NeverTypeConstructor.Instance;
    #endregion

    PlainType? TryConstructNullaryPlainType(BarePlainType? containingType);

    /// <summary>
    /// Attempt to construct a type from this type constructor with possibly unknown arguments. If
    /// any argument is unknown, the result is <see langword="null"/>.
    /// </summary>
    BareType? TryConstruct(BareType? containingType, IFixedList<IMaybeType> arguments);

    Type? TryConstructNullaryType(BareType? containingType);
}
