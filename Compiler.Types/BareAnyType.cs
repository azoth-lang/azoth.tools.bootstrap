using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class BareAnyType : BareReferenceType
{
    #region Singleton
    internal static readonly BareAnyType Instance = new();

    private BareAnyType() { }
    #endregion

    public override TypeName Name => SpecialTypeName.Any;

    public override ReferenceType With(ReferenceCapability capability)
        => new AnyType(capability);

    #region Equals
    public override bool Equals(BareReferenceType? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => Name.GetHashCode();
    #endregion

    public override string ToString() => Name.ToString();
}
