using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface IBlockOrResultSyntax
{
    IPromise<DataType?> DataType { get; }
}
