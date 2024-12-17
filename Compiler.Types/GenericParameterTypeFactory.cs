using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public class GenericParameterTypeFactory : ITypeFactory
{
    private const string? NoContainingTypeMessage = "Generic parameter types imply the containing type and should be constructed without one.";

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
    PlainType ITypeFactory.TryConstructNullaryPlainType(ConstructedPlainType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), NoContainingTypeMessage);
        return PlainType;
    }

    BareType? ITypeFactory.TryConstruct(BareType? containingType, IFixedList<IMaybeType> arguments)
    {
        Requires.Null(containingType, nameof(containingType), NoContainingTypeMessage);
        TypeRequires.NoArgs(arguments, nameof(arguments));
        // There is no bare type for a generic parameter
        return null;
    }

    Type? ITypeFactory.TryConstructNullaryType(BareType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), NoContainingTypeMessage);
        return Type;
    }
    #endregion
}
