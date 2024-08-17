using Azoth.Tools.Bootstrap.Compiler.Core.Code;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface IMethodSelfParameterSyntax
{
    public static IMethodSelfParameterSyntax Create(TextSpan span, bool isLentBinding, ICapabilityConstraintSyntax capability)
        // TODO allow AG to specify that name is always null
        => Create(span, null, isLentBinding, capability);
}
