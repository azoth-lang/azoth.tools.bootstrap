using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(IExpressionType), typeof(IMaybeType))]
public interface IMaybeExpressionType : IEquatable<IMaybeExpressionType>
{
    public sealed DataType AsType => (DataType)this;

    /// <summary>
    /// Convert types for constant values to their corresponding types.
    /// </summary>
    IMaybeType ToNonConstValueType();

    IMaybeExpressionAntetype ToAntetype();

    /// <summary>
    /// How this type would be written in source code.
    /// </summary>
    string ToSourceCodeString();

    /// <summary>
    /// How this type would be written in IL.
    /// </summary>
    string ToILString();
}
