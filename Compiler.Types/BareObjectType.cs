using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class BareObjectType : BareReferenceType
{
    public BareObjectType(NamespaceName containingNamespace, TypeName name, bool isConst)
    {
        ContainingNamespace = containingNamespace;
        Name = name;
        IsConst = isConst;
    }

    // TODO this needs a containing package
    public NamespaceName ContainingNamespace { get; }
    public TypeName Name { get; }
    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConst { get; }

    /// <summary>
    /// Make a version of this type for use as the constructor parameter. One issue is
    /// that it should be mutable even if the underlying type is declared immutable.
    /// </summary>
    /// <remarks>This is always `mut` because the type can be mutated inside the constructor.</remarks>
    public ObjectType ToConstructorSelf()
        // TODO does this need to be `init`?
        => With(ReferenceCapability.Mutable);

    /// <summary>
    /// Make a version of this type for use as the return type of the default constructor.
    /// </summary>
    /// <remarks>This is always either `iso` or `const` depending on whether the type was declared
    /// with `const` because there are no parameters that could break the new objects isolation.</remarks>
    public ObjectType ToDefaultConstructorReturn()
        => With(IsConst ? ReferenceCapability.Constant : ReferenceCapability.Isolated);

    public ObjectType With(ReferenceCapability capability)
        => ObjectType.Create(this, capability);

    /// <summary>
    /// Make a version of this type that is the default read reference capability for the type. That
    /// is either read-only or constant.
    /// </summary>
    public ObjectType WithRead()
        => With(IsConst ? ReferenceCapability.Constant : ReferenceCapability.ReadOnly);
}
