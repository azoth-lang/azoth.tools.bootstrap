using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface IBlockOrResultSyntax
{
    DataType? DataType { get; }
}