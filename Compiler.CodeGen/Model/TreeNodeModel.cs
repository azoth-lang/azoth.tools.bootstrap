using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public class TreeNodeModel
{
    public TreeModel Tree { get; }
    public TreeNodeSyntax Syntax { get; }

    public InternalSymbol Defines { get; }
    public SymbolType DefinesType { get; }
    public IFixedSet<Symbol> Supertypes { get; }
    private readonly Lazy<IFixedSet<TreeNodeModel>> supertypeNodes;
    public IFixedSet<TreeNodeModel> SupertypeNodes => supertypeNodes.Value;
    public IFixedSet<TreeNodeModel> AncestorNodes => ancestorNodes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> ancestorNodes;
    public IFixedSet<TreeNodeModel> ChildNodes => childNodes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> childNodes;

    /// <summary>
    /// Whether this tree node is abstract meaning that it cannot be instantiated directly.
    /// </summary>
    /// <remarks>Right now, this is determined solely by whether this node has child nodes. If
    /// needed, keywords <c>abstract</c> and <c>concrete</c> could be added to the definition file
    /// to allow overriding this.</remarks>
    public bool IsAbstract => !ChildNodes.IsEmpty;

    public IFixedSet<TreeNodeModel> DescendantNodes => descendantNodes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> descendantNodes;

    public IFixedList<Property> DeclaredProperties { get; }

    /// <summary>
    /// Properties inherited from the parents of a rule. If the same property is defined on multiple
    /// parents, it will be listed multiple times.
    /// </summary>
    public IFixedList<Property> InheritedProperties => inheritedProperties.Value;
    private readonly Lazy<IFixedList<Property>> inheritedProperties;

    /// <summary>
    /// Get all properties for a rule. If that rule defines the property itself, that
    /// is the one definition. When the rule doesn't define the property, base classes are
    /// recursively searched for definitions. Multiple definitions are returned when multiple
    /// parents of a rule contain definitions of the property without it being defined on that rule.
    /// </summary>
    public IFixedList<Property> AllProperties => allProperties.Value;
    private readonly Lazy<IFixedList<Property>> allProperties;

    public TreeNodeModel(TreeModel tree, TreeNodeSyntax syntax)
    {
        Tree = tree;
        Syntax = syntax;
        Defines = Symbol.CreateInternalFromSyntax(tree, syntax.Defines);
        DefinesType = new SymbolType(Defines);
        Supertypes = syntax.Supertypes.Select(s => Symbol.CreateFromSyntax(tree, s)).ToFixedSet();
        if (Tree.Root is { } root && root != Defines && !Supertypes.Any(s => s is InternalSymbol))
            Supertypes = Supertypes.Append(root).ToFixedSet();

        supertypeNodes = new(() => Supertypes.OfType<InternalSymbol>().Select(s => s.ReferencedNode)
                                             .EliminateRedundantRules().ToFixedSet());
        ancestorNodes = new(() => SupertypeNodes.Concat(SupertypeNodes.SelectMany(p => p.AncestorNodes)).ToFixedSet());
        childNodes = new(() => Tree.Nodes.Where(r => r.SupertypeNodes.Contains(this)).ToFixedSet());
        descendantNodes = new(() => ChildNodes.Concat(ChildNodes.SelectMany(r => r.DescendantNodes)).ToFixedSet());

        DeclaredProperties = syntax.DeclaredProperties.Select(p => new Property(this, p)).ToFixedList();
        inheritedProperties = new(() => SupertypeNodes.SelectMany(r => r.AllProperties).Distinct().ToFixedList());
        allProperties = new(() =>
        {
            var rulePropertyNames = DeclaredProperties.Select(p => p.Name).ToFixedSet();
            return DeclaredProperties
                   .Concat(InheritedProperties.Where(p => !rulePropertyNames.Contains(p.Name)))
                   .ToFixedList();
        });
    }

    public IEnumerable<Property> InheritedPropertiesNamed(Property property)
        => InheritedPropertiesNamed(property.Name);
    public IEnumerable<Property> InheritedPropertiesNamed(string propertyName)
        => InheritedProperties.Where(p => p.Name == propertyName);

    public IEnumerable<Property> InheritedPropertiesWithoutMostSpecificImplementationNamed(Property property)
        => InheritedPropertiesWithoutMostSpecificImplementationNamed(property.Name);
    public IEnumerable<Property> InheritedPropertiesWithoutMostSpecificImplementationNamed(string propertyName)
    {
        var inheritedProperties = InheritedPropertiesNamed(propertyName).ToFixedSet();
        if (inheritedProperties.Count <= 1)
            return [];

        return inheritedProperties
               .SelectMany(p => p.Node.InheritedPropertiesNamed(propertyName))
               .Distinct()
               .Except(inheritedProperties);
    }
}
