using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(IType), typeof(Type), typeof(ConstValueType))]
public interface IExpressionType : IMaybeExpressionType, IPseudotype
{
    public new sealed Type AsType => (Type)this;
}
