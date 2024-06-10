using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class GenericParameterAntetype : NonGenericNominalAntetype, INonVoidAntetype
{
    public UserDeclaredGenericAntetype DeclaringAntetype { get; }
    public AntetypeGenericParameter Parameter { get; }
    public override IdentifierName Name => Parameter.Name;
    // TODO this should be based on generic constraints
    public override IFixedSet<NominalAntetype> Supertypes => FixedSet.Empty<NominalAntetype>();

    // TODO is this right?
    public bool HasReferenceSemantics => true;

    public GenericParameterAntetype(UserDeclaredGenericAntetype declaringAntetype, AntetypeGenericParameter parameter)
    {
        DeclaringAntetype = declaringAntetype;
        Parameter = parameter;
    }

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is GenericParameterAntetype that
               && DeclaringAntetype.Equals(that.DeclaringAntetype)
               && Name.Equals(that.Name);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaringAntetype, Name);
    #endregion

    public override string ToString() => $"{DeclaringAntetype}.{Parameter.Name}";
}
