using System;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(IType), typeof(DataType))]
public interface IMaybeType : IEquatable<IMaybeType>
{
    #region Standard Types
    /// <summary>
    /// The unknown type as <see cref="DataType"/>.
    /// </summary>
    /// <remarks>There are places where the compiler cannot infer the expression type. This can be
    /// used to force the compiler to use <see cref="DataType"/>.</remarks>
    public static readonly DataType Unknown = UnknownType.Instance;
    #endregion

    /// <summary>
    /// How this type would be written in source code.
    /// </summary>
    string ToSourceCodeString();

    /// <summary>
    /// How this type would be written in IL.
    /// </summary>
    string ToILString();
}
