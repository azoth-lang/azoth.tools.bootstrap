using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// Not all types have a <see cref="BareTypeConstructor"/>, yet there are places in the compiler where
/// the ability to construct a type is needed even if it doesn't have a one. A <see cref="ITypeFactory"/>
/// provides that ability.
/// </summary>
[Closed(
    typeof(BareTypeConstructor),
    typeof(VoidTypeFactory),
    typeof(NeverTypeFactory),
    typeof(GenericParameterTypeFactory))]
public interface ITypeFactory
{
    #region Standard
    public static readonly ITypeFactory Void = VoidTypeFactory.Instance;
    public static readonly ITypeFactory Never = NeverTypeFactory.Instance;
    #endregion

    PlainType? TryConstructNullaryPlainType(BarePlainType? containingType);

    /// <summary>
    /// Attempt to construct a type from this type constructor with possibly unknown arguments. If
    /// any argument is unknown, the result is <see langword="null"/>.
    /// </summary>
    BareType? TryConstruct(BareType? containingType, IFixedList<IMaybeType> arguments);

    Type? TryConstructNullaryType(BareType? containingType);
}
