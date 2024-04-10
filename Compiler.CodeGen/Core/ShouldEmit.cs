using System.Diagnostics;
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
    public static bool Property(Rule rule, Property property)
        => property.IsDeclared;

    public static bool NewRule(Rule rule)
        => rule.IsNew;

    public static bool Class(Rule rule)
        => rule.IsTerminal && !rule.Defines.Syntax.IsQuoted;

    public static bool AmendedRule(Rule rule)
        => !rule.IsNew;

    public static bool Constructor(Rule rule)
        => rule.IsTerminal;

    public static bool ExtendsSupertype(Rule rule)
    {
        if (rule.Defines.Name == "CompilationUnit")
            Debugger.Break();
        return !rule.DescendantsModified;
    }
}
