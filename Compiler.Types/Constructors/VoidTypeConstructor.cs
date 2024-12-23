using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

internal sealed class VoidTypeConstructor : ITypeConstructor
{
    #region Singleton
    internal static readonly VoidTypeConstructor Instance = new VoidTypeConstructor();

    private VoidTypeConstructor() { }
    #endregion

    public PlainType TryConstructNullaryPlainType(BarePlainType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), "Void does not have a containing type.");
        return PlainType.Void;
    }

    public BareType? TryConstruct(BareType? containingType, IFixedList<IMaybeType> arguments)
    {
        Requires.Null(containingType, nameof(containingType), "Void does not have a containing type.");
        TypeRequires.NoArgs(arguments, nameof(arguments));
        return null;
    }

    public Type? TryConstructNullaryType(BareType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), "Void does not have a containing type.");
        return Type.Void;
    }
}
