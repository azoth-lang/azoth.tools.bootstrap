using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class Property
{
    public static IEqualityComparer<Property> NameAndTypeComparer { get; }
        = EqualityComparer<Property>.Create((p1, p2) => p1!.Name == p2!.Name && p1.Type == p2.Type,
            p => HashCode.Combine(p.Name, p.Type));

    public static IEqualityComparer<Property> NameAndTypeEquivalenceComparer { get; }
        = EqualityComparer<Property>.Create(
            (p1, p2) => p1?.Name == p2?.Name && Types.Type.EquivalenceComparer.Equals(p1?.Type, p2?.Type),
            p => HashCode.Combine(p.Name, Types.Type.EquivalenceComparer.GetHashCode(p.Type)));

    public PropertySyntax Syntax { get; }

    public TreeNodeModel Node { get; }
    public string Name => Syntax.Name;
    public NonVoidType Type { get; }
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
    /// Is the type of this property a reference to another node?
    /// </summary>
    public bool ReferencesNode => Type.UnderlyingSymbol is InternalSymbol { ReferencedNode: not null };

    public Property(TreeNodeModel node, PropertySyntax syntax)
    {
        Node = node;
        Syntax = syntax;

        var type = Types.Type.CreateFromSyntax(Node.Tree, syntax.Type);
        if (type is not NonVoidType nonVoidType)
            throw new InvalidOperationException("Property type must be a non-void type.");
        Type = nonVoidType;
        isNewDefinition = new(() => node.InheritedPropertiesNamed(this).Any());
        isDeclared = new(() =>
        {
            var baseProperties = node.InheritedPropertiesNamed(this).ToList();
            return baseProperties.Count != 1 || !Types.Type.AreEquivalent(baseProperties[0].Type, Type);
        });
    }

    public override string ToString() => $"{Name}:{Type}";
}
