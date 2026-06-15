using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class MethodClausesAspect
{
    public static partial AccessModifier? OverridesOrHides_AccessModifier(IOverridesOrHidesNode node)
        => node.Syntax.AccessModifier?.ToAccessModifier();
}
