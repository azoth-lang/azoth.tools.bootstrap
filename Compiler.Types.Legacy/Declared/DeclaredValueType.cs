using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

[Closed(typeof(SimpleType), typeof(StructType))]
public abstract class DeclaredValueType : DeclaredType
{
    private protected DeclaredValueType(bool isConstType, IFixedList<GenericParameter> genericParameters)
        : base(isConstType, genericParameters) { }

    public abstract override BareNonVariableType With(IFixedList<IType> typeArguments);
}
