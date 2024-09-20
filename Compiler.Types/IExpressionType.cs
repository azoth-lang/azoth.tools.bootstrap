using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(IType), typeof(Type), typeof(ConstValueType))]
public interface IExpressionType : IMaybeExpressionType
{
    public new sealed Type AsType => (Type)this;
}
