using System;
using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// Object types are the types created with class and trait declarations. An
/// object type may have generic parameters that may be filled with generic
/// arguments. An object type with generic parameters but no generic arguments
/// is an *unbound type*. One with generic arguments supplied for all
/// parameters is *a constructed type*. One with some but not all arguments
/// supplied is *partially constructed type*.
/// </summary>
/// <remarks>
/// There will be two special object types `Type` and `Metatype`
/// </remarks>
public sealed class ObjectType : ReferenceType
{
    public new BareObjectType BareType => (BareObjectType)base.BareType;
    // TODO this needs a containing package
    public NamespaceName ContainingNamespace => BareType.ContainingNamespace;
    public TypeName Name => BareType.Name;
    public override bool IsKnown { [DebuggerStepThrough] get => true; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConst => BareType.IsConst;

    /// <summary>
    /// Create a object type for a given class or trait.
    /// </summary>
    public static ObjectType Create(
            NamespaceName containingNamespace,
            TypeName name,
            bool isConst,
            ReferenceCapability capability)
        => new(new BareObjectType(containingNamespace, name, isConst), capability);

    /// <summary>
    /// Create a object type for a given class or trait.
    /// </summary>
    public static ObjectType Create(
        BareObjectType bareType,
        ReferenceCapability capability)
        => new(bareType, capability);

    private ObjectType(
        BareObjectType bareType,
        ReferenceCapability capability)
        : base(capability, bareType)
    {
    }

    public override string ToSourceCodeString()
    {
        var builder = new StringBuilder();
        if (Capability != ReferenceCapability.ReadOnly)
        {
            builder.Append(Capability.ToSourceString());
            builder.Append(' ');
        }
        BareType.ToString(builder);
        return builder.ToString();
    }

    public override string ToILString()
    {
        var builder = new StringBuilder();
        builder.Append(Capability.ToILString());
        builder.Append(' ');
        BareType.ToString(builder);
        return builder.ToString();
    }

    public bool DeclaredTypesEquals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ObjectType otherType
               && ContainingNamespace == otherType.ContainingNamespace
               && Name == otherType.Name;
    }

    #region Equality
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ObjectType otherType
               && ContainingNamespace == otherType.ContainingNamespace
               && Name == otherType.Name
               && Capability == otherType.Capability;
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingNamespace, Name, Capability);
    #endregion

    public override ObjectType To(ReferenceCapability referenceCapability)
        => new(BareType, referenceCapability);

    /// <remarks>For constant types, there can still be read only references. For example, inside
    /// the constructor.</remarks>
    public override ObjectType WithoutWrite() => (ObjectType)base.WithoutWrite();
}
