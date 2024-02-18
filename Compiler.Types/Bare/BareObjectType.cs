using System;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// An object type without a reference capability.
/// </summary>
public sealed class BareObjectType : BareReferenceType
{
    public override DeclaredReferenceType DeclaredType { get; }

    public static BareObjectType Create(
        DeclaredReferenceType declaredType,
        FixedList<DataType> typeArguments)
        => new(declaredType, typeArguments);

    private BareObjectType(DeclaredReferenceType declaredType, FixedList<DataType> typeArguments)
        : base(declaredType, typeArguments)
    {
        DeclaredType = declaredType;
    }

    public override BareObjectType AccessedVia(ReferenceCapability capability)
    {
        if (DeclaredType.GenericParameters.All(p => p.Variance != Variance.Independent))
            return this;
        var newTypeArguments = DeclaredType.GenericParameters
            .Zip(TypeArguments, (p, arg) => p.Variance == Variance.Independent ? arg.AccessedVia(capability) : arg)
            .ToFixedList();
        return Create(DeclaredType, newTypeArguments);
    }

    public override ObjectType With(ReferenceCapability capability)
        => ObjectType.Create(capability, this);

    public ObjectTypeConstraint With(ReferenceCapabilityConstraint capability)
        => new(capability, this);

    #region Equality
    public override bool Equals(BareReferenceType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BareReferenceType otherType
            && DeclaredType == otherType.DeclaredType
            && TypeArguments.Equals(otherType.TypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaredType, TypeArguments);
    #endregion

    public override string ToSourceCodeString() => ToString(t => t.ToSourceCodeString());

    public override string ToILString() => ToString(t => t.ToILString());

    private string ToString(Func<DataType, string> toString)
    {
        var builder = new StringBuilder();
        builder.Append(DeclaredType.ContainingNamespace);
        if (DeclaredType.ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(DeclaredType.Name.ToBareString());
        if (TypeArguments.Any())
        {
            builder.Append('[');
            builder.AppendJoin(", ", TypeArguments.Select(toString));
            builder.Append(']');
        }
        return builder.ToString();
    }
}
