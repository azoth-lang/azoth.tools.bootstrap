using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

[Closed(typeof(SimpleType), typeof(StructType))]
public abstract class DeclaredValueType : DeclaredType
{
    private protected DeclaredValueType(bool isConstType, IFixedList<GenericParameterType> genericParametersTypes)
        : base(isConstType, genericParametersTypes) { }

    public abstract override BareValueType With(IFixedList<DataType> typeArguments);
}
