using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

/// <summary>
/// A reference type as it is declared without any capability or type arguments
/// </summary>
[Closed(
    typeof(ObjectType),
    typeof(AnyType))]
public abstract class DeclaredReferenceType : DeclaredType
{
    public bool IsAbstract { get; }

    public override TypeSemantics Semantics => TypeSemantics.Reference;

    private protected DeclaredReferenceType(
        bool isDeclaredConst,
        bool isAbstract,
        IFixedList<GenericParameterType> genericParametersTypes)
        : base(isDeclaredConst, genericParametersTypes)
    {
        IsAbstract = isAbstract;
    }

    public abstract override BareReferenceType With(IFixedList<DataType> typeArguments);

    public abstract override CapabilityType With(ReferenceCapability capability, IFixedList<DataType> typeArguments);
}
