using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

internal sealed class VoidTypeFactory : ITypeFactory
{
    #region Singleton
    internal static readonly VoidTypeFactory Instance = new VoidTypeFactory();

    private VoidTypeFactory() { }
    #endregion

    public BareType? TryConstruct(IFixedList<IMaybeType> arguments)
    {
        TypeRequires.NoArgs(arguments, nameof(arguments));
        return null;
    }
}
