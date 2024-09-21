using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The type of an Azoth expression. This can be either a regular type or a constant value type.
/// </summary>
[Closed(typeof(IType), typeof(Type), typeof(ConstValueType))]
public interface IExpressionType : IMaybeExpressionType, IPseudotype;
