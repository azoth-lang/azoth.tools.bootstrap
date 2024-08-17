using Azoth.Tools.Bootstrap.Compiler.Core.Code;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface IInitializerSelfParameterSyntax
{
    public static IInitializerSelfParameterSyntax Create(TextSpan span, bool isLentBinding, ICapabilitySyntax capability)
        // TODO allow AG to specify that name is always null
        => Create(span, null, isLentBinding, capability);
}
