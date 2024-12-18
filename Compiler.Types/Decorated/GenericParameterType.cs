using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// TODO maybe this should only be constructed from the GenericParameterPlainType to avoid duplicate instances?
// e.g. T
// NOTE: generic parameters are the only plain types that do not need a capability
// TODO make TypeConstructor.Parameter the factory!
public sealed class GenericParameterType : NonVoidType
{
    public override GenericParameterPlainType PlainType { get; }

    public IdentifierName Name => PlainType.Name;

    public TypeConstructor.Parameter Parameter => PlainType.Parameter;

    internal override GenericParameterTypeReplacements BareTypeReplacements => GenericParameterTypeReplacements.None;

    public override bool HasIndependentTypeArguments => false;

    /// <remarks>Use <see cref="GenericParameterTypeFactory.Type"/> instead of directly using this.</remarks>
    internal GenericParameterType(GenericParameterPlainType plainType)
    {
        PlainType = plainType;
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
