using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The type introduced by a generic parameter.
/// </summary>
public sealed class GenericParameterPlainType : NonVoidPlainType
{
    public OrdinaryTypeConstructor DeclaringTypeConstructor { [DebuggerStepThrough] get; }
    public override TypeSemantics? Semantics => null;
    public TypeConstructorParameter Parameter { [DebuggerStepThrough] get; }
    public IdentifierName Name => Parameter.Name;

    // TODO generic parameters can have supertypes based on generic constraints

    /// <remarks>Use <see cref="GenericParameterTypeConstructor.PlainType"/> instead of directly using this.</remarks>
    internal GenericParameterPlainType(
        OrdinaryTypeConstructor declaringTypeConstructor,
        TypeConstructorParameter parameter)
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
