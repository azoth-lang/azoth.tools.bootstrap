using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public class GenericParameterTypeConstructor : ITypeConstructor
{
    private const string? NoContainingTypeMessage = "Generic parameter types imply the containing type and should be constructed without one.";

    public OrdinaryTypeConstructor DeclaringTypeConstructor { [DebuggerStepThrough] get; }
    public TypeConstructorParameter Parameter { [DebuggerStepThrough] get; }
    public GenericParameterPlainType PlainType { [DebuggerStepThrough] get; }
    public GenericParameterType Type { [DebuggerStepThrough] get; }

    internal GenericParameterTypeConstructor(
        OrdinaryTypeConstructor declaringTypeConstructor,
        TypeConstructorParameter parameter)
    {
        DeclaringTypeConstructor = declaringTypeConstructor;
        Parameter = parameter;
        PlainType = new(DeclaringTypeConstructor, Parameter);
        Type = new(PlainType);
    }

    #region ITypeConstructor implementation
    PlainType ITypeConstructor.TryConstructNullaryPlainType(BarePlainType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), NoContainingTypeMessage);
        return PlainType;
    }

    BareType? ITypeConstructor.TryConstruct(BareType? containingType, IFixedList<IMaybeType> arguments)
    {
        Requires.Null(containingType, nameof(containingType), NoContainingTypeMessage);
        TypeRequires.NoArgs(arguments, nameof(arguments));
        // There is no bare type for a generic parameter
        return null;
    }

    Type ITypeConstructor.TryConstructNullaryType(BareType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), NoContainingTypeMessage);
        return Type;
    }
    #endregion
}
