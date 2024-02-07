using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface ISelfExpressionSyntax
{
    [DisallowNull] Pseudotype? Pseudotype { get; set; }
}
