using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface ITypeSyntax
{
    [DisallowNull] DataType? NamedType { get; set; }
}
