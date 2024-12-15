using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

internal class NeverTypeFactory : ITypeFactory
{
    #region Singleton
    internal static readonly NeverTypeFactory Instance = new NeverTypeFactory();

    private NeverTypeFactory() { }
    #endregion

    public PlainType TryConstructNullaryPlainType() => PlainType.Never;

    public BareType? TryConstruct(IFixedList<IMaybeType> arguments)
    {
        TypeRequires.NoArgs(arguments, nameof(arguments));
        return null;
    }

    public Type TryConstructNullaryType() => Type.Never;
}
