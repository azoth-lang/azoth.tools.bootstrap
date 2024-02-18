using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

[Closed(typeof(DeclaredReferenceType), typeof(DeclaredValueType))]
public abstract class DeclaredType
{
    public static readonly DeclaredAnyType Any = DeclaredAnyType.Instance;

    public abstract SimpleName? ContainingPackage { get; }
    public abstract NamespaceName ContainingNamespace { get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConstType { get; }

    public abstract TypeName Name { get; }

    public FixedList<GenericParameter> GenericParameters { get; }
    public bool HasIndependentGenericParameters { get; }
    public bool AllowsVariance { get; }
    public FixedList<GenericParameterType> GenericParameterTypes { get; }

    // TODO this is really awkward. There should be a subtype relationship
    public FixedList<DataType> GenericParameterDataTypes { get; }
    public abstract FixedSet<BareReferenceType> Supertypes { get; }

    protected DeclaredType(
        bool isConstType,
        FixedList<GenericParameterType> genericParametersTypes)
    {
        IsConstType = isConstType;
        GenericParameters = genericParametersTypes.Select(t => t.Parameter).ToFixedList();
        HasIndependentGenericParameters = GenericParameters.Any(p => p.Variance == Variance.Independent);
        AllowsVariance = GenericParameters.Any(p => p.Variance != Variance.Invariant);
        GenericParameterTypes = genericParametersTypes;
        GenericParameterDataTypes = GenericParameterTypes.ToFixedList<DataType>();
    }

    public abstract BareType With(FixedList<DataType> typeArguments);

    public abstract CapabilityType With(ReferenceCapability capability, FixedList<DataType> typeArguments);

    public abstract override string ToString();
}
