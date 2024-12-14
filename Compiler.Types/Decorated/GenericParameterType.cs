using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// TODO maybe this should only be constructed from the GenericParameterPlainType to avoid duplicate instances?
// e.g. T
// NOTE: generic parameters are the only plain types that do not need a capability
public sealed class GenericParameterType : NonVoidType, ITypeFactory
{
    public override GenericParameterPlainType PlainType { get; }

    public IdentifierName Name => PlainType.Name;

    public TypeConstructor.Parameter Parameter => PlainType.Parameter;

    public override TypeReplacements TypeReplacements => TypeReplacements.None;

    public override bool HasIndependentTypeArguments => false;

    public GenericParameterType(GenericParameterPlainType plainType)
    {
        PlainType = plainType;
    }

    BareType? ITypeFactory.TryConstruct(IFixedList<IMaybeType> arguments)
    {
        TypeRequires.NoArgs(arguments, nameof(arguments));
        // There is no bare type for a generic parameter
        return null;
    }

    #region Equality
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is GenericParameterType otherType
               && PlainType.Equals(otherType.PlainType);
    }

    public override int GetHashCode() => HashCode.Combine(PlainType);
    #endregion

    public override string ToSourceCodeString() => PlainType.ToString();

    public override string ToILString() => PlainType.ToString();
}
