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
    // TODO this needs a containing package
    public NamespaceName ContainingNamespace { get; }
    public TypeName Name { get; }
    public override bool IsKnown { [DebuggerStepThrough] get => true; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConst { get; }

    /// <summary>
    /// Create a object type for a given class or trait.
    /// </summary>
    public static ObjectType Create(
            NamespaceName containingNamespace,
            TypeName name,
            bool isConst,
            ReferenceCapability capability)
        => new(containingNamespace, name, isConst, capability);

    private ObjectType(
        NamespaceName containingNamespace,
        TypeName name,
        bool isConst,
        ReferenceCapability capability)
        : base(capability)
    {
        ContainingNamespace = containingNamespace;
        Name = name;
        IsConst = isConst;
    }

    /// <summary>
    /// Make a version of this type for use as the constructor parameter. One issue is
    /// that it should be mutable even if the underlying type is declared immutable.
    /// </summary>
    /// <remarks>This is always `mut` because the type can be mutated inside the constructor.</remarks>
    public ObjectType ToConstructorSelf()
        // TODO does this need to be `init`?
        => To(ReferenceCapability.Mutable);

    /// <summary>
    /// Make a version of this type for use as the return type of the default constructor.
    /// </summary>
    /// <remarks>This is always either `iso` or `const` depending on whether the type was declared
    /// with `const` because there are no parameters that could break the new objects isolation.</remarks>
    public ObjectType ToDefaultConstructorReturn()
        => To(IsConst ? ReferenceCapability.Constant : ReferenceCapability.Isolated);

    public override string ToSourceCodeString()
    {
        var builder = new StringBuilder();
        if (Capability != ReferenceCapability.ReadOnly)
        {
            builder.Append(Capability.ToSourceString());
            builder.Append(' ');
        }
        builder.Append(ContainingNamespace);
        if (ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(Name);
        return builder.ToString();
    }

    public override string ToILString()
    {
        var builder = new StringBuilder();
        builder.Append(Capability.ToILString());
        builder.Append(' ');
        builder.Append(ContainingNamespace);
        if (ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(Name);
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
        => new(ContainingNamespace, Name, IsConst, referenceCapability);

    /// <remarks>For constant types, there can still be read only references. For example, inside
    /// the constructor.</remarks>
    public override ObjectType WithoutWrite() => (ObjectType)base.WithoutWrite();
}
