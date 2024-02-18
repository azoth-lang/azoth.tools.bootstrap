using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

public abstract class DeclaredValueType : DeclaredType
{
    protected DeclaredValueType(bool isConstType, FixedList<GenericParameterType> genericParametersTypes)
        : base(isConstType, genericParametersTypes) { }
}
