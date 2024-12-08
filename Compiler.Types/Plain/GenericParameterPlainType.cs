using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The type introduced by a generic parameter.
/// </summary>
public sealed class GenericParameterPlainType : VariablePlainType
{
    public OrdinaryTypeConstructor DeclaringTypeConstructor { get; }
    public TypeConstructor.Parameter Parameter { get; }
    public override IdentifierName Name => Parameter.Name;
    // TODO this should be based on generic constraints
    public override IFixedSet<ConstructedPlainType> Supertypes => IPlainType.AnySet;

    public GenericParameterPlainType(
        OrdinaryTypeConstructor declaringTypeConstructor,
        TypeConstructor.Parameter parameter)
    {
        DeclaringTypeConstructor = declaringTypeConstructor;
        Parameter = parameter;
    }

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
}
