using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

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
    public static bool Property(Grammar grammar, GrammarRule rule, GrammarProperty property)
    {
        var baseProperties = grammar.InheritedProperties(rule, property.Name).ToList();
        return baseProperties.Count != 1 || baseProperties[0].Type != property.Type;
    }
}