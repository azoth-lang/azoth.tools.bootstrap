using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

[Closed(typeof(SimpleType))]
public abstract class DeclaredValueType : DeclaredType
{
    private protected DeclaredValueType(bool isConstType, FixedList<GenericParameterType> genericParametersTypes)
        : base(isConstType, genericParametersTypes) { }

    public abstract override BareValueType With(FixedList<DataType> typeArguments);
}
