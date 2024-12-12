using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The type introduced by a generic parameter.
/// </summary>
public sealed class GenericParameterPlainType : ConstructedOrVariablePlainType
{
    public OrdinaryTypeConstructor DeclaringTypeConstructor { get; }
    public override TypeConstructor? TypeConstructor => null;
    public override TypeSemantics? Semantics => null;
    public TypeConstructor.Parameter Parameter { get; }
    public override IdentifierName Name => Parameter.Name;

    public override bool AllowsVariance => false;
    // TODO this should be based on generic constraints
    public override IFixedSet<ConstructedPlainType> Supertypes => IPlainType.AnySet;

    internal override PlainTypeReplacements TypeReplacements => PlainTypeReplacements.None;

    public GenericParameterPlainType(
        OrdinaryTypeConstructor declaringTypeConstructor,
        TypeConstructor.Parameter parameter)
    {
        DeclaringTypeConstructor = declaringTypeConstructor;
        Parameter = parameter;
    }

    public override IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType) => plainType;

    #region Equality
    public override bool Equals(IMaybePlainType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is GenericParameterPlainType that
               && DeclaringTypeConstructor.Equals(that.DeclaringTypeConstructor)
               && Name.Equals(that.Name);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaringTypeConstructor, Name);
    #endregion

    public override string ToString() => $"{DeclaringTypeConstructor}.{Parameter.Name}";

    public override string ToBareString() => ToString();
}
