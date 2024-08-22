using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// The semantic model for an attribute.
/// </summary>
[Closed(typeof(AspectAttributeModel), typeof(PropertyModel))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class AttributeModel : IMemberModel
{
    public static IEqualityComparer<AttributeModel> NameAndTypeComparer { get; }
        = EqualityComparer<AttributeModel>.Create((p1, p2) => p1?.Name == p2?.Name && p1?.Type == p2?.Type,
            p => HashCode.Combine(p.Name, p.Type));

    public static IEqualityComparer<AttributeModel> NameComparer { get; }
        = EqualityComparer<AttributeModel>.Create((p1, p2) => p1?.Name == p2?.Name, p => HashCode.Combine(p.Name));

    public abstract AttributeSyntax? Syntax { get; }
    public abstract TreeNodeModel Node { get; }
    public abstract string Name { get; }
    public abstract bool IsMethod { get; }
    public abstract TypeModel Type { get; }
    public virtual bool IsSyncLockRequired => false;

    /// <summary>
    /// Something is a new definition if it replaces some parent definition.
    /// </summary>
    public bool IsNewDefinition => isNewDefinition.Value;
    private readonly Lazy<bool> isNewDefinition;

    /// <summary>
    /// Whether this attribute needs to be declared in the node interface.
    /// </summary>
    /// <remarks>
    /// A property needs declared under three conditions:
    /// 1. there is no definition of the property in the parent
    /// 2. the single parent definition has a different type or parameters
    /// 3. the property is defined in multiple parents, in that case it is
    ///    ambiguous unless it is redefined in the current interface.
    /// 4. The parent attribute is a different kind of attribute
    /// </remarks>
    public bool IsDeclarationRequired => isDeclarationRequired.Value;
    private readonly Lazy<bool> isDeclarationRequired;

    /// <summary>
    /// Is the type of this property a reference to another node?
    /// </summary>
    public bool ReferencesNode => Type.UnderlyingSymbol is InternalSymbol;

    protected AttributeModel()
    {
        isNewDefinition = new(() => Node.InheritedAttributesNamedSameAs(this).Any());
        isDeclarationRequired = new(() =>
        {
            var baseProperty = Node.InheritedAttributesNamedSameAs(this).TrySingle();
            return baseProperty is null // There were none or multiple, so it needs to be declared
                   || baseProperty.Type != Type
                   || baseProperty.IsMethod != IsMethod
                   || baseProperty.GetType() != GetType();
        });
    }

    public abstract override string ToString();
}
