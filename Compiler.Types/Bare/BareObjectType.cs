using System;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// An object type without a reference capability.
/// </summary>
public sealed class BareObjectType : BareReferenceType
{
    public override DeclaredObjectType DeclaredType { get; }

    public static BareObjectType Create(
        DeclaredObjectType declaredType,
        FixedList<DataType> typeArguments)
        => new(declaredType, typeArguments);

    private BareObjectType(DeclaredObjectType declaredType, FixedList<DataType> typeArguments)
        : base(declaredType, typeArguments)
    {
        DeclaredType = declaredType;
    }

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
