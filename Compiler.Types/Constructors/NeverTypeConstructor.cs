using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

internal class NeverTypeConstructor : ITypeConstructor
{
    #region Singleton
    internal static readonly NeverTypeConstructor Instance = new NeverTypeConstructor();

    private NeverTypeConstructor() { }
    #endregion

    public PlainType TryConstructNullaryPlainType(BarePlainType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), "Never does not have a containing type.");
        return PlainType.Never;
    }

    public BareType? TryConstruct(BareType? containingType, IFixedList<IMaybeType> arguments)
    {
        Requires.Null(containingType, nameof(containingType), "Never does not have a containing type.");
        TypeRequires.NoArgs(arguments, nameof(arguments));
        return null;
    }

    public Type TryConstructNullaryType(BareType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), "Never does not have a containing type.");
        return Type.Never;
    }
}
