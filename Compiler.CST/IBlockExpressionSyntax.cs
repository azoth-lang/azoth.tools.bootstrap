using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

/// <summary>
/// A block expression. Not to be used to represent function or type bodies.
/// </summary>
public partial interface IBlockExpressionSyntax
{
    [DisallowNull] new DataType? DataType { get; set; }
}
