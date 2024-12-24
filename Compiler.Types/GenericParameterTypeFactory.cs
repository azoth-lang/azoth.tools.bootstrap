using System.Diagnostics;
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

    public OrdinaryTypeConstructor DeclaringTypeConstructor { [DebuggerStepThrough] get; }
    public TypeConstructorParameter Parameter { [DebuggerStepThrough] get; }
    public GenericParameterPlainType PlainType { [DebuggerStepThrough] get; }
    public GenericParameterType Type { [DebuggerStepThrough] get; }

    internal GenericParameterTypeFactory(
        OrdinaryTypeConstructor declaringTypeConstructor,
        TypeConstructorParameter parameter)
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
