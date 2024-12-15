using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public abstract class AssociatedTypeFactory : ITypeFactory
{
    public abstract AssociatedPlainType PlainType { get; }
    public abstract AssociatedBareType BareType { get; }

    PlainType? ITypeFactory.TryConstructNullaryPlainType() => throw new NotImplementedException();

    BareType? ITypeFactory.TryConstruct(IFixedList<IMaybeType> arguments) => throw new NotImplementedException();

    Type? ITypeFactory.TryConstructNullaryType() => throw new NotImplementedException();
}
