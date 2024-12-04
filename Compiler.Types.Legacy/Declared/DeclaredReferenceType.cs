using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

/// <summary>
/// A reference type as it is declared without any capability or type arguments
/// </summary>
[Closed(
    typeof(ObjectType),
    typeof(AnyType))]
public abstract class DeclaredReferenceType : DeclaredType
{
    public bool IsAbstract { get; }

    /// <summary>
    /// Whether this type is a `class` (as opposed to a `trait`)
    /// </summary>
    public bool IsClass { get; }

    private protected DeclaredReferenceType(
        bool isDeclaredConst,
        bool isAbstract,
        bool isClass,
        IFixedList<GenericParameter> genericParameter)
        : base(isDeclaredConst, genericParameter)
    {
        IsAbstract = isAbstract;
        IsClass = isClass;
    }

    public abstract override BareReferenceType With(IFixedList<IType> typeArguments);

    public abstract override CapabilityType With(Capability capability, IFixedList<IType> typeArguments);
}
