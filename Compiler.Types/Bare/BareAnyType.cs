using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// The `Any` type without a reference capability.
/// </summary>
public sealed class BareAnyType : BareReferenceType
{
    #region Singleton
    internal static readonly BareAnyType Instance = new();

    private BareAnyType() : base(DeclaredReferenceType.Any, FixedList<DataType>.Empty) { }
    #endregion

    public override DeclaredAnyType DeclaredType => DeclaredReferenceType.Any;

    public override BareReferenceType AccessedVia(ReferenceCapability capability) => this;
    public override AnyType With(ReferenceCapability capability) => new AnyType(capability);

    #region Equality
    public override bool Equals(BareReferenceType? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(SpecialTypeName.Any);
    #endregion

    public override string ToILString() => SpecialTypeName.Any.ToString();

    public override string ToSourceCodeString() => SpecialTypeName.Any.ToString();
}
