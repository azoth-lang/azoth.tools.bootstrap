using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class PropertyModel
{
    public static IEqualityComparer<PropertyModel> NameAndTypeComparer { get; }
        = EqualityComparer<PropertyModel>.Create((p1, p2) => p1?.Name == p2?.Name && p1?.Type == p2?.Type,
            p => HashCode.Combine(p.Name, p.Type));

    public static IEqualityComparer<PropertyModel> NameComparer { get; }
        = EqualityComparer<PropertyModel>.Create((p1, p2) => p1?.Name == p2?.Name,
            p => HashCode.Combine(p.Name));

    public PropertySyntax? Syntax { get; }

    public TreeNodeModel Node { get; }
    public string Name { get; }
    public NonVoidType Type { get; }
    /// <summary>
    /// Something is a new definition if it replaces some parent definition.
    /// </summary>
    public bool IsNewDefinition => isNewDefinition.Value;
    private readonly Lazy<bool> isNewDefinition;

    /// <summary>
    /// Whether this property is declared in the node interface.
    /// </summary>
    /// <remarks>
    /// A property needs declared under three conditions:
    /// 1. there is no definition of the property in the parent
    /// 2. the single parent definition has a different type
    /// 3. the property is defined in multiple parents, in that case it is
    ///    ambiguous unless it is redefined in the current interface.
    /// </remarks>
    public bool IsDeclarationRequired => isDeclarationRequired.Value;
    private readonly Lazy<bool> isDeclarationRequired;

    /// <summary>
    /// Is the type of this property a reference to another node?
    /// </summary>
    public bool ReferencesNode => Type.UnderlyingSymbol is InternalSymbol { ReferencedNode: not null };

    public PropertyModel(TreeNodeModel node, PropertySyntax syntax)
    {
        Node = node;
        Syntax = syntax;
        Name = Syntax.Name;

        var type = Types.Type.CreateFromSyntax(Node.Tree, syntax.Type);
        if (type is not NonVoidType nonVoidType)
            throw new InvalidOperationException("Property type must be a non-void type.");
        Type = nonVoidType;
        isNewDefinition = new(() => node.InheritedPropertiesNamedSameAs(this).Any());
        isDeclarationRequired = new(() =>
        {
            var baseProperties = node.InheritedPropertiesNamedSameAs(this).ToList();
            return baseProperties.Count != 1 || baseProperties[0].Type != Type;
        });
    }

    public PropertyModel(TreeNodeModel node, string name, NonVoidType type)
    {
        Node = node;
        Name = name;
        Type = type;

        isNewDefinition = new(() => node.InheritedPropertiesNamedSameAs(this).Any());
        isDeclarationRequired = new(() =>
        {
            var baseProperties = node.InheritedPropertiesNamedSameAs(this).ToList();
            return baseProperties.Count != 1 || baseProperties[0].Type != Type;
        });
    }

    public override string ToString() => $"{Node.Defines}.{Name}:{Type}";
}
