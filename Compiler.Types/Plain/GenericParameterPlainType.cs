using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The type introduced by a generic parameter.
/// </summary>
public sealed class GenericParameterPlainType : VariablePlainType
{
    public OrdinaryTypeConstructor DeclaringAntetype { get; }
    public TypeConstructorParameter Parameter { get; }
    public override IdentifierName Name => Parameter.Name;
    // TODO this should be based on generic constraints
    public override IFixedSet<NamedPlainType> Supertypes => FixedSet.Empty<NamedPlainType>();

    public GenericParameterPlainType(OrdinaryTypeConstructor declaringAntetype, TypeConstructorParameter parameter)
    {
        DeclaringAntetype = declaringAntetype;
        Parameter = parameter;
    }

    #region Equality
    public override bool Equals(IMaybeAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is GenericParameterPlainType that
               && DeclaringAntetype.Equals(that.DeclaringAntetype)
               && Name.Equals(that.Name);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaringAntetype, Name);
    #endregion

    public override string ToString() => $"{DeclaringAntetype}.{Parameter.Name}";
}
