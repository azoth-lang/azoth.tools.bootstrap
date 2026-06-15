using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class MethodClausesAspect
{
    public static partial InheritanceRelationship OverridesOrHides_InheritanceRelationship(IOverridesOrHidesNode node)
        => node.Syntax.OverridesOrHidesToken switch
        {
            IOverridesKeywordToken _ => InheritanceRelationship.Overrides,
            IHidesKeywordToken _ => InheritanceRelationship.Hides,
            _ => throw ExhaustiveMatch.Failed(node.Syntax.OverridesOrHidesToken)
        };

    public static partial AccessModifier? OverridesOrHides_AccessModifier(IOverridesOrHidesNode node)
        => node.Syntax.AccessModifier?.ToAccessModifier();
}
