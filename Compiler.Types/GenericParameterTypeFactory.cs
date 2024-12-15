using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public class GenericParameterTypeFactory : ITypeFactory
{
    public OrdinaryTypeConstructor DeclaringTypeConstructor { get; }
    public TypeConstructor.Parameter Parameter { get; }
    public GenericParameterPlainType PlainType { get; }
    public GenericParameterType Type { get; }

    internal GenericParameterTypeFactory(
        OrdinaryTypeConstructor declaringTypeConstructor,
        TypeConstructor.Parameter parameter)
    {
        DeclaringTypeConstructor = declaringTypeConstructor;
        Parameter = parameter;
        PlainType = new(DeclaringTypeConstructor, Parameter);
        Type = new(PlainType);
    }

    #region ITypeFactory implementation
    PlainType ITypeFactory.TryConstructNullaryPlainType() => PlainType;

    BareType? ITypeFactory.TryConstruct(IFixedList<IMaybeType> arguments)
    {
        TypeRequires.NoArgs(arguments, nameof(arguments));
        // There is no bare type for a generic parameter
        return null;
    }

    Type? ITypeFactory.TryConstructNullaryType() => Type;
    #endregion
}
