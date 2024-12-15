using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

// TODO replace with VoidType?
internal sealed class VoidTypeFactory : ITypeFactory
{
    #region Singleton
    internal static readonly VoidTypeFactory Instance = new VoidTypeFactory();

    private VoidTypeFactory() { }
    #endregion

    public PlainType TryConstructNullaryPlainType() => PlainType.Void;

    public BareType? TryConstruct(IFixedList<IMaybeType> arguments)
    {
        TypeRequires.NoArgs(arguments, nameof(arguments));
        return null;
    }

    public Type? TryConstructNullaryType() => Type.Void;
}
