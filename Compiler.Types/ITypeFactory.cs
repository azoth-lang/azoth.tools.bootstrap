using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// Not all types have a <see cref="TypeConstructor"/>, yet there are places in the compiler where
/// the ability to construct a type is needed even if it doesn't have a one. A <see cref="ITypeFactory"/>
/// provides that ability.
/// </summary>
public interface ITypeFactory
{
    #region Standard
    public static readonly ITypeFactory Void = VoidTypeFactory.Instance;
    public static readonly ITypeFactory Never = NeverTypeFactory.Instance;
    #endregion

    /// <summary>
    /// Attempt to construct a type from this type constructor with possibly unknown arguments. If
    /// any argument is unknown, the result is <see langword="null"/>.
    /// </summary>
    BareType? TryConstruct(IFixedList<IMaybeType> arguments);
}
