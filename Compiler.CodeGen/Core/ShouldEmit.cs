using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

public static class ShouldEmit
{
    /// <summary>
    /// A property needs declared under three conditions:
    /// 1. there is no definition of the property in the parent
    /// 2. the single parent definition has a different type
    /// 3. the property is defined in multiple parents, in that case it is
    ///    ambiguous unless it is redefined in the current interface.
    /// </summary>
    public static bool Property(Property property)
        => property.IsDeclared;

    public static bool Class(Rule rule)
        => rule.IsTerminal;

    public static bool Constructor(Rule rule)
        => rule.IsTerminal;

    public static bool ExtendsSupertype(Rule rule)
        => !rule.DescendantsModified;

    public static bool RunExplicitImplementation(Pass pass)
        => pass.From is null || pass.FromContext is null
        || pass.To is null || pass.ToContext is null;
}
