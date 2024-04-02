using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class Property
{
    public PropertyNode Syntax { get; }

    public Rule Rule { get; }
    public string Name => Syntax.Name;
    public Type Type { get; }
    /// <summary>
    /// Something is a new definition if it replaces some parent definition.
    /// </summary>
    public bool IsNewDefinition => isNewDefinition.Value;
    private readonly Lazy<bool> isNewDefinition;

    /// <summary>
    /// Whether this property is declared in the rule interface.
    /// </summary>
    /// <remarks>
    /// A property needs declared under three conditions:
    /// 1. there is no definition of the property in the parent
    /// 2. the single parent definition has a different type
    /// 3. the property is defined in multiple parents, in that case it is
    ///    ambiguous unless it is redefined in the current interface.
    /// </remarks>
    public bool IsDeclared => isDeclared.Value;
    private readonly Lazy<bool> isDeclared;

    /// <summary>
    /// Is the type of this property a reference to another rule?
    /// </summary>
    public bool ReferencesRule => Type.Symbol.ReferencedRule is not null;

    public Property(Rule rule, PropertyNode syntax)
    {
        Rule = rule;
        Syntax = syntax;

        Type = new Type(this, syntax.Type);
        isNewDefinition = new(() => rule.InheritedPropertiesNamed(this).Any());
        isDeclared = new(() =>
        {
            var baseProperties = rule.InheritedPropertiesNamed(this).ToList();
            return baseProperties.Count != 1 || !baseProperties[0].Type.IsEquivalentTo(Type);
        });
    }
}
